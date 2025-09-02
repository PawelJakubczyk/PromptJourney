using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyPromtHistory;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;

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

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<MidjourneyPromptHistory>(error);
            }

            return await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistoryResult.Value);
        }
    }
}