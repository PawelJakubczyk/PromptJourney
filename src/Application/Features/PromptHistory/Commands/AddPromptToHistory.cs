using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.PromptHistory.Responses;
using Domain.Entities;
using Domain.Errors;
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

            List<DomainError> domainErrors = [];
            
            domainErrors
                .CollectErrors(prompt)
                .CollectErrors(version);

            var validationErrors = CreateValidationErrorIfAny<PromptHistoryResponse>
            (
                (nameof(domainErrors), domainErrors)
            );
            
            if (validationErrors is not null) return validationErrors;

            var history = MidjourneyPromptHistory.Create(prompt, version);
            var addResult = await _promptHistoryRepository.AddPromptToHistoryAsync(history.Value);
            var persistenceErrors = addResult.Errors;

            validationErrors = CreateValidationErrorIfAny<PromptHistoryResponse>
            (
                (nameof(persistenceErrors), persistenceErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var response = PromptHistoryResponse.FromDomain(addResult.Value);

            return Result.Ok(response);
        }
    }
}