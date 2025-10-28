using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.PromptHistory.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.PromptHistory.Queries;

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
            var result = await WorkflowPipeline
                .EmptyAsync()
                .Congregate(pipeline => pipeline
                    .IfHistoryLimitNotGreaterThanZero(query.Count)
                    .IfHistoryCountExceedsAvailable(query.Count, _promptHistoryRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _promptHistoryRepository
                    .GetLastHistoryRecordsAsync(query.Count, cancellationToken))
                .MapResult<List<MidjourneyPromptHistory>, List<PromptHistoryResponse>>
                    (promptHistoryList => [.. promptHistoryList.Select(PromptHistoryResponse.FromDomain)]);

            return result;
        }
    }
}