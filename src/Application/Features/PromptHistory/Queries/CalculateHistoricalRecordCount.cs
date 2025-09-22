using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using FluentResults;

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
            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .ExecuteAndMapResultIfNoErrors(
                    () => _promptHistoryRepository.CalculateHistoricalRecordCountAsync(cancellationToken),
                    count => count
                );

            return result;
        }

    }
}