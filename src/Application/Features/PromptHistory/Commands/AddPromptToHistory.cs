using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.PromptHistory.Responses;
using Domain.Entities.MidjourneyPromtHistory;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.PromptHistory.Commands;

public static class AddPromptToHistory
{
    public sealed record Command(string Prompt, string Version) : ICommand<PromptHistoryResponse>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, PromptHistoryResponse>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<PromptHistoryResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var prompt = Prompt.Create(command.Prompt);
            var version = ModelVersion.Create(command.Version);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(version.Value, _versionRepository);

            var promptHistoryResult = MidjourneyPromptHistory.Create
            (
                prompt.Value,
                version.Value
            );

            var domainErrors = promptHistoryResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<PromptHistoryResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _promptHistoryRepository.AddPromptToHistoryAsync(promptHistoryResult.Value);

            if (result.IsFailed)
                return Result.Fail<PromptHistoryResponse>(result.Errors);

            var response = PromptHistoryResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}