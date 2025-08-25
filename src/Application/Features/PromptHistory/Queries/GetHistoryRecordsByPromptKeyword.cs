using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;

namespace Application.Features.PromptHistory.Queries;

public static class GetHistoryRecordsByPromptKeyword
{
    public sealed record Query(string Keyword) : IQuery<List<MidjourneyPromptHistory>>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : IQueryHandler<Query, List<MidjourneyPromptHistory>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<MidjourneyPromptHistory>>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.History.Input.Keyword.MustNotBeNullOrEmpty(query.Keyword);

            return await _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(query.Keyword);
        }
    }
}