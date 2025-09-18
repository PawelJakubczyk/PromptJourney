using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.PromptHistory.Responses;
using FluentResults;
using Domain.ValueObjects;
using Domain.Errors;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.PromptHistory.Queries;

public static class GetHistoryRecordsByPromptKeyword
{
    public sealed record Query(string Keyword) : IQuery<List<PromptHistoryResponse>>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository
    ) : IQueryHandler<Query, List<PromptHistoryResponse>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<PromptHistoryResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var keyword = Keyword.Create(query.Keyword);

            List<DomainError> domainErrors = [];
            domainErrors.CollectErrors(keyword);

            var validationErrors = CreateValidationErrorIfAny<List<PromptHistoryResponse>>
            (
                (nameof(domainErrors), domainErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var result = await _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword.Value);
            var persistenceErrors = result.Errors;

            validationErrors = CreateValidationErrorIfAny<List<PromptHistoryResponse>>
            (
                (nameof(persistenceErrors), persistenceErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var responses = result.Value
                .Select(PromptHistoryResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}