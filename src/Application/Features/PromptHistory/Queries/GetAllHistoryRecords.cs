using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.PromptHistory.Responses;
using FluentResults;

namespace Application.Features.PromptHistory.Queries;

public static class GetAllHistoryRecords
{
    public sealed record Query : IQuery<List<PromptHistoryResponse>>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : IQueryHandler<Query, List<PromptHistoryResponse>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<PromptHistoryResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _promptHistoryRepository.GetAllHistoryRecordsAsync();

            if (result.IsFailed)
                return Result.Fail<List<PromptHistoryResponse>>(result.Errors);

            var responses = result.Value
                .Select(PromptHistoryResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}