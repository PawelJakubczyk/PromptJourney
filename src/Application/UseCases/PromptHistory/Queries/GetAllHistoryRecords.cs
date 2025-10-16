using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.PromptHistory.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.PromptHistory.Queries;

public static class GetAllHistoryRecords
{
    public sealed record Query : IQuery<List<PromptHistoryResponse>>
    {
        public static readonly Query Singletone = new();
    };

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
                .ExecuteIfNoErrors(() => _promptHistoryRepository
                    .GetAllHistoryRecordsAsync(cancellationToken))
                .MapResult<List<MidjourneyPromptHistory>, List<PromptHistoryResponse>>
                    (promptHistoryList => [.. promptHistoryList.Select(PromptHistoryResponse.FromDomain)]);

            return result;
        }
    }
}
