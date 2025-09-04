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

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : IQueryHandler<Query, List<PromptHistoryResponse>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<PromptHistoryResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfHistoryLimitNotGreaterThanZero(query.Count)
                .IfHistoryCountExceedsAvailable(query.Count, _promptHistoryRepository);

            var validationErrors = CreateValidationErrorIfAny<List<PromptHistoryResponse>>(applicationErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _promptHistoryRepository.GetLastHistoryRecordsAsync(query.Count);

            if (result.IsFailed)
                return Result.Fail<List<PromptHistoryResponse>>(result.Errors);

            var responses = result.Value
                .Select(PromptHistoryResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}