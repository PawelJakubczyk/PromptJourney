using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.PromptHistory.Queries;

public static class CalculateHistoricalRecordCount
{
    public sealed record Query() : IQuery<int>;

    public sealed class Handler
    (
        IPromptHistoryRepository promptHistoryRepository
    ) : IQueryHandler<Query, int>
    {
        private readonly IPromptHistoryRepository _promptHistoryRepository = promptHistoryRepository;

        public async Task<Result<int>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _promptHistoryRepository.CalculateHistoricalRecordCountAsync();
            var persistenceErrors = result.Errors;

            var validationErrors = CreateValidationErrorIfAny<int>
            (
                (nameof(persistenceErrors), persistenceErrors)
            );
            
            if (validationErrors is not null) return validationErrors;

            return Result.Ok(result.Value);
        }
    }
}