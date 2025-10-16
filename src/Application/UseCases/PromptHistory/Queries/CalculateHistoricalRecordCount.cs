using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;

namespace Application.UseCases.PromptHistory.Queries;

public static class CalculateHistoricalRecordCount
{
    public sealed record Query : IQuery<int>
    {
        public static readonly Query Singletone = new();
    };

public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository
    ) : IQueryHandler<Query, int>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<int>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _promptHistoryRepository
                    .CalculateHistoricalRecordCountAsync(cancellationToken);

            return result;
        }
    }
}
