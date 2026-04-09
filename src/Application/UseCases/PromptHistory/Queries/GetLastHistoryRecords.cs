using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.PromptHistory.Responses;
using Domain.Entities;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.PromptHistory.Queries;

public static class GetLastHistoryRecords
{
    public sealed record Query(int? Count) : IQuery<List<PromptHistoryResponse>>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository
    ) : IQueryHandler<Query, List<PromptHistoryResponse>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<PromptHistoryResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var count = query.Count ?? 1;

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CongregateErrors(
                    pipeline => pipeline.IfHistoryRecordsLimitNotGreaterThanZero(count),
                    pipeline => pipeline.IfHistoryCountExceedsAvailable(count, _promptHistoryRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _promptHistoryRepository
                    .GetLastHistoryRecordsAsync(count, cancellationToken))
                .MapResult<List<MidjourneyPromptHistory>, List<PromptHistoryResponse>>
                    (promptHistoryList => [.. promptHistoryList.Select(PromptHistoryResponse.FromDomain)]);

            return result;
        }
    }
}