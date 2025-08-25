using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;

namespace Application.Features.PromptHistory.Queries;

public static class GetLastHistoryRecords
{
    public sealed record Query(int Count) : IQuery<List<MidjourneyPromptHistory>>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : IQueryHandler<Query, List<MidjourneyPromptHistory>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<MidjourneyPromptHistory>>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.History.LimitMustBeGreaterThanZero(query.Count);
            await Validate.History.CountMustNotExceedHistoricalRecords(query.Count, _promptHistoryRepository);

            return await _promptHistoryRepository.GetLastHistoryRecordsAsync(query.Count);
        }
    }
}