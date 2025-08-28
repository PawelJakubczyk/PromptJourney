using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.ExampleLinks.Queries;

public static class GetExampleLinksByStyle
{
    public sealed record Query(StyleName Style) : IQuery<List<MidjourneyStyleExampleLink>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository
    ) : IQueryHandler<Query, List<MidjourneyStyleExampleLink>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<MidjourneyStyleExampleLink>>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.Style.ShouldExists(query.Style, _styleRepository);

            return await _exampleLinkRepository.GetExampleLinksByStyleAsync(query.Style);
        }
    }
}