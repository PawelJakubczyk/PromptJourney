using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyPromtHistory;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.PromptHistory.Commands;

public static class AddPromptToHistory
{
    public sealed record Command(Prompt Prompt, ModelVersion Version) : ICommand<MidjourneyPromptHistory>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, MidjourneyPromptHistory>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyPromptHistory>> Handle(Command command, CancellationToken cancellationToken)
        {
            var promptHistory = MidjourneyPromptHistory.Create
            (
                command.Prompt,
                command.Version
            );

            await Validate.Version.ShouldExists(command.Version, _versionRepository);

            return await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory.Value);
        }
    }
}