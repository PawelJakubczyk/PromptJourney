using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.PromptHistory.Commands;

public static class AddPromptToHistory
{
    public sealed record Command(string? HistoryId, string? Prompt, string? Version) : ICommand<string>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, string>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<string>> Handle(Command command, CancellationToken cancellationToken)
        {
            var historyId = command.HistoryId == null ? HistoryID.Create() : HistoryID.Create(command.HistoryId);
            var prompt = Prompt.Create(command.Prompt);
            var version = ModelVersion.Create(command.Version);
            var createdOn = CreatedOn.Create(DateTime.UtcNow.ToString());

            var promptHistory = MidjourneyPromptHistory.Create(historyId, prompt, version, createdOn);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(promptHistory)
                .IfVersionNotExists(version, _versionRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _promptHistoryRepository
                    .AddPromptToHistoryAsync(promptHistory.Value, cancellationToken))
                .MapResult<MidjourneyPromptHistory, string>
                    (history => history.HistoryId.ToString());

            return result;
        }
    }
}
