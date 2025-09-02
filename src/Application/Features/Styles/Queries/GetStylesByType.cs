using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyStyles;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Styles.Queries;

public static class GetStylesByType
{
    public sealed record Query(StyleType StyleType) : IQuery<List<MidjourneyStyle>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<MidjourneyStyle>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<MidjourneyStyle>>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<StyleType>(query.StyleType);

            var validationErrors = CreateValidationErrorIfAny<List<MidjourneyStyle>>(domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _styleRepository.GetStylesByTypeAsync(query.StyleType);
        }
    }
}