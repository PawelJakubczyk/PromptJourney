using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.PromptHistory.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.PromptHistory.Queries;

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
                .ExecuteIfNoErrors(() => _promptHistoryRepository
                    .GetHistoryByDateRangeAsync(query.From, query.To, cancellationToken))
                .MapResult<List<MidjourneyPromptHistory>, List<PromptHistoryResponse>>
                    (promptHistoryList => [.. promptHistoryList.Select(PromptHistoryResponse.FromDomain)]);

            return result;
        }
    }
}