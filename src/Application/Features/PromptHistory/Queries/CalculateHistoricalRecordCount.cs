using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;

namespace Application.Features.PromptHistory.Queries;

public static class CalculateHistoricalRecordCount
{
    public sealed record Query() : IQuery<int>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository) : IQueryHandler<Query, int>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<int>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _promptHistoryRepository.CalculateHistoricalRecordCountAsync();
        }
    }
}