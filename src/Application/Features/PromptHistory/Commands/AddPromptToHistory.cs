using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;

namespace Application.Features.PromptHistory.Commands;

public static class AddPromptToHistory
{
    public sealed record Command(
        string Prompt,
        string Style,
        string Version,
        DateTime CreatedAt
    ) : ICommand<MidjourneyPromptHistory>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : ICommandHandler<Command, MidjourneyPromptHistory>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<MidjourneyPromptHistory>> Handle(Command command, CancellationToken cancellationToken)
        {
            var promptHistory = MidjourneyPromptHistory.Create
            (
                command.Prompt,
                command.Version
            );

            return await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistory.Value);
        }
    }
}