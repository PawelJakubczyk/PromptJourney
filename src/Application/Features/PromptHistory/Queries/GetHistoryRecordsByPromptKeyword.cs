using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;
using Domain.ValueObjects;
using Domain.Errors;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Features.PromptHistory.Queries;

public static class GetHistoryRecordsByPromptKeyword
{
    public sealed record Query(Keyword Keyword) : IQuery<List<MidjourneyPromptHistory>>;

    public sealed class Handler(IPromptHistoryRepository promptHistoryRepository)
        : IQueryHandler<Query, List<MidjourneyPromptHistory>>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<List<MidjourneyPromptHistory>>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<Keyword>(query.Keyword);

            if (domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<List<MidjourneyPromptHistory>>(error);
            }

            return await _promptHistoryRepository.GetHistoryRecordsByPromptKeywordAsync(query.Keyword);
        }
    }
}