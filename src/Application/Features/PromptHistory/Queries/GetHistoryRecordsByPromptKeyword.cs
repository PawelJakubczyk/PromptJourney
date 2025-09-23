using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.PromptHistory.Responses;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

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

            var result = await ValidationPipeline
                .EmptyAsync()
                .CollectErrors(keyword)
                .IfNoErrors()
                    .Executes(() => _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(keyword.Value, cancellationToken))
                        .MapResult(domainList => domainList.Select(PromptHistoryResponse.FromDomain).ToList());

            return result;
        }
    }
}