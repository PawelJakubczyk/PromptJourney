using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.PromptHistory.Responses;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.PromptHistory.Queries;

public static class GetLastHistoryRecords
{
    public sealed record Query(int Count) : IQuery<List<PromptHistoryResponse>>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository
    ) : IQueryHandler<Query, List<PromptHistoryResponse>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<PromptHistoryResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfHistoryLimitNotGreaterThanZero(query.Count)
                .IfHistoryCountExceedsAvailable(query.Count, _promptHistoryRepository);

            var validationErrors = CreateValidationErrorIfAny<List<PromptHistoryResponse>>
            (
                (nameof(applicationErrors), applicationErrors)
            );
            
            if (validationErrors is not null) return validationErrors;

            var lastRecordResult = await _promptHistoryRepository.GetLastHistoryRecordsAsync(query.Count);
            var persistenceErrors = lastRecordResult.Errors;

            validationErrors = CreateValidationErrorIfAny<List<PromptHistoryResponse>>
            (
                (nameof(persistenceErrors), persistenceErrors)
            );
            
            if (validationErrors is not null) return validationErrors;

            var responses = lastRecordResult.Value
                .Select(PromptHistoryResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}