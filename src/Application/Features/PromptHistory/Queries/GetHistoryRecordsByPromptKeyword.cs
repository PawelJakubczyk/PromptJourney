using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.PromptHistory.Responses;
using FluentResults;
using Domain.ValueObjects;
using Application.Extension;

namespace Application.Features.PromptHistory.Queries;

public static class GetHistoryRecordsByPromptKeyword
{
    public sealed record Query(string Keyword) : IQuery<List<PromptHistoryResponse>>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository
    ) : IQueryHandler<Query, List<PromptHistoryResponse>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<PromptHistoryResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var keyword = Keyword.Create(query.Keyword);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(keyword)
                .ExecuteAndMapResultIfNoErrors(
                    () => _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword.Value, cancellationToken),
                    domainList => domainList.Select(PromptHistoryResponse.FromDomain).ToList()
                );

            return result;
        }
    }
}