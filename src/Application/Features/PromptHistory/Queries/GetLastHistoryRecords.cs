using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.PromptHistory.Responses;
using FluentResults;
using Utilities.Validation;

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
            var result = await WorkflowPipeline
                .EmptyAsync()
                    .Validate(pipeline => pipeline
                        .IfHistoryLimitNotGreaterThanZero(query.Count)
                        .IfHistoryCountExceedsAvailable(query.Count, _promptHistoryRepository, cancellationToken))
                    .ExecuteIfNoErrors(() => _promptHistoryRepository.GetLastHistoryRecordsAsync(query.Count, cancellationToken))
                    .MapResult(domainList => domainList.Select(PromptHistoryResponse.FromDomain).ToList());

            return result;
        }
    }
}