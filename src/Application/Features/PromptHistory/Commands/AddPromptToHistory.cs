using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyPromtHistory;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Application.Errors.ErrorsExtensions;

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
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(command.Version, _versionRepository);

            var promptHistoryResult = MidjourneyPromptHistory.Create
            (
                command.Prompt,
                command.Version
            );

            var domainErrors = promptHistoryResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<MidjourneyPromptHistory>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistoryResult.Value);
        }
    }
}