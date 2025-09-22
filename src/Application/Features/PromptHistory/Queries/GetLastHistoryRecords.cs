using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.PromptHistory.Responses;
using FluentResults;

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
            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .IfHistoryLimitNotGreaterThanZero(query.Count)
                .IfHistoryCountExceedsAvailable(query.Count, _promptHistoryRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors(
                    () => _promptHistoryRepository.GetLastHistoryRecordsAsync(query.Count, cancellationToken),
                    domainList => domainList.Select(PromptHistoryResponse.FromDomain).ToList()
                );

            return result;
        }
    }
}