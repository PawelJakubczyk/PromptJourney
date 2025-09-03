using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Queries;

public static class GetStylesByDescriptionKeyword
{
    public sealed record Query(string DescriptionKeyword) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var keyword = Keyword.Create(query.DescriptionKeyword);

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<Keyword>(keyword);

            var validationErrors = CreateValidationErrorIfAny<List<StyleResponse>>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _styleRepository.GetStylesByDescriptionKeywordAsync(keyword.Value);

            if (result.IsFailed)
                return Result.Fail<List<StyleResponse>>(result.Errors);

            var responses = result.Value
                .Select(StyleResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}