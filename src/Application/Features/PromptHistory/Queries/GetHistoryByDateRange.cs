using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.PromptHistory.Responses;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.PromptHistory.Queries;

public static class GetHistoryByDateRange
{
    public sealed record Query(DateTime From, DateTime To) : IQuery<List<PromptHistoryResponse>>;

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
                        .IfDateInFuture(query.From)
                        .IfDateInFuture(query.To)
                        .IfDateRangeNotChronological(query.From, query.To))
                        .ExecuteIfNoErrors(() => _promptHistoryRepository.GetHistoryByDateRangeAsync(query.From, query.To, cancellationToken))
                            .MapResult
                            (
                                domainList => domainList
                                .Select(PromptHistoryResponse.FromDomain)
                                .ToList()
                            );

            return result;
        }

    }
}