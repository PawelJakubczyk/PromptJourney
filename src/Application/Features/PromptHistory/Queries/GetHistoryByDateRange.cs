using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;

namespace Application.Features.PromptHistory.Queries;

public static class GetHistoryByDateRange
{
    public sealed record Query(DateTime From, DateTime To) : IQuery<List<MidjourneyPromptHistory>>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : IQueryHandler<Query, List<MidjourneyPromptHistory>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<MidjourneyPromptHistory>>> Handle(Query query, CancellationToken cancellationToken)
        {
            //Validate.Date.RangeShouldBeChronological(query.From, query.To);
            //Validate.Date.ShouldNotBeInFuture(query.To);

            return await _promptHistoryRepository.GetHistoryByDateRangeAsync(query.From, query.To);
        }
    }
}