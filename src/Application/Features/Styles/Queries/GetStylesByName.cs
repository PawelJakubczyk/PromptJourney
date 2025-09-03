using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Queries;

public static class GetStylesByName
{
    public sealed record Query(string StyleName) : IQuery<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            List<DomainError> domainErrors = [];
            domainErrors
                .CollectErrors<StyleName>(styleName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(styleName.Value, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<StyleResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _styleRepository.GetStyleByNameAsync(styleName.Value);

            if (result.IsFailed)
                return Result.Fail<StyleResponse>(result.Errors);

            var response = StyleResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}