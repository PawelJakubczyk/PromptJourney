using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.PromptHistory.Responses;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.PromptHistory.Queries;

public static class GetAllHistoryRecords
{
    public sealed record Query : IQuery<List<PromptHistoryResponse>>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository
    ) : IQueryHandler<Query, List<PromptHistoryResponse>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<PromptHistoryResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _promptHistoryRepository.GetAllHistoryRecordsAsync();
            var persistenceErrors = result.Errors;

            var validationErrors = CreateValidationErrorIfAny<List<PromptHistoryResponse>>
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