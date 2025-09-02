using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Queries;

public static class GetStylesByName
{
    public sealed record Query(StyleName StyleName) : IQuery<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];
            domainErrors
                .CollectErrors<StyleName>(query.StyleName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(query.StyleName, _styleRepository);

            var validationErrors = CreateValidationErrorIfAny<MidjourneyStyle>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _styleRepository.GetStyleByNameAsync(query.StyleName);
        }
    }
}