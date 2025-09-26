using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.PromptHistory.Queries;

public static class CalculateHistoricalRecordCount
{
    public sealed record Query() : IQuery<int>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository
    ) : IQueryHandler<Query, int>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<int>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _promptHistoryRepository.CalculateHistoricalRecordCountAsync(cancellationToken))
                .MapResult(count => count);

            return result;
        }

    }
}