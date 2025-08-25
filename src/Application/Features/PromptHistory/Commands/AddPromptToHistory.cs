using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;

namespace Application.Features.PromptHistory.Commands;

public static class AddPromptToHistory
{
    public sealed record Command(string Prompt, string Version) : ICommand<MidjourneyPromptHistory>;

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

            await Validate.History.Input.MustNotBeNullOrEmpty(command.Version);
            await Validate.History.Input.MustHaveMaximumLength(command.Version);

            await Validate.Version.Input.CannotBeNullOrEmpty(command.Version);
            await Validate.Version.Input.MustHaveMaximumLength(command.Version);
            await Validate.Version.MustExists(command.Version, _versionRepository);

            return await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory.Value);
        }
    }
}