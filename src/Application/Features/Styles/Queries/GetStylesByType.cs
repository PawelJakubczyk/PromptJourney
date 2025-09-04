using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Styles.Responses;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.Styles.Queries;

public static class GetStylesByType
{
    public sealed record Query(string StyleType) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleType = StyleType.Create(query.StyleType);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleType>(styleType);

            var validationErrors = CreateValidationErrorIfAny<List<StyleResponse>>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _styleRepository.GetStylesByTypeAsync(styleType.Value);

            if (result.IsFailed)
                return Result.Fail<List<StyleResponse>>(result.Errors);

            var responses = result.Value
                .Select(StyleResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}