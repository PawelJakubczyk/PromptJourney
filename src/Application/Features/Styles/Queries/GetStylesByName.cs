using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;

namespace Application.Features.Styles.Queries;

public static class GetStylesByName
{
    public sealed record Query(StyleName StyleName) : IQuery<MidjourneyStyle>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, MidjourneyStyle>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<MidjourneyStyle>> Handle(Query query, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfStyleNotExists(query.StyleName, _styleRepository);

            return await _styleRepository.GetStyleByNameAsync(query.StyleName);
        }
    }
}