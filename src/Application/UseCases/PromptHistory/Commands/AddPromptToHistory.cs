using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.PromptHistory.Commands;

public static class AddPromptToHistory
{
    public sealed record Command(string Prompt, string Version) : ICommand<string>;

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
            var prompt = Prompt.Create(command.Prompt);
            var version = ModelVersion.Create(command.Version);

            var promptHistory = MidjourneyPromptHistory.Create(prompt, version);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(promptHistory)
                .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _promptHistoryRepository
                    .AddPromptToHistoryAsync(promptHistory.Value, cancellationToken))
                .MapResult<MidjourneyPromptHistory, string>
                    (history => history.HistoryId.ToString());

            return result;
        }
    }
}
