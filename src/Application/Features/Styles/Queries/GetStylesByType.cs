using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.Styles.Queries;

public static class GetStylesByType
{
    public sealed record Query(string Type) : IQuery<List<MidjourneyStyle>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<MidjourneyStyle>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<MidjourneyStyle>>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _styleRepository.GetStylesByTypeAsync(query.Type);
        }
    }
}