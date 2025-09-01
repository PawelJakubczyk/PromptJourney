using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.Styles.Queries;

public static class GetStylesByDescriptionKeyword
{
    public sealed record Query(Keyword DescriptionKeyword) : IQuery<List<MidjourneyStyle>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<MidjourneyStyle>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<MidjourneyStyle>>> Handle(Query query, CancellationToken cancellationToken)
        {
                return await _styleRepository.GetStylesByDescriptionKeywordAsync(query.DescriptionKeyword);
        }
    }
}