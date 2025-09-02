using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Application.Errors.ErrorsExtensions;

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
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfDateRangeNotChronological(query.From, query.To)
                .IfDateInFuture(query.To);

            var validationErrors = CreateValidationErrorIfAny<List<MidjourneyPromptHistory>>(applicationErrors);
            if (validationErrors is not null) return validationErrors;

            return await _promptHistoryRepository.GetHistoryByDateRangeAsync(query.From, query.To);
        }
    }
}