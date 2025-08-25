using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.ExampleLinks.Queries;

public static class GetExampleLinksByStyleAndVersion
{
    public sealed record Query(string Style, string Version) : IQuery<List<MidjourneyStyleExampleLink>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository,
        IVersionRepository versionRepository
    ) : IQueryHandler<Query, List<MidjourneyStyleExampleLink>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<MidjourneyStyleExampleLink>>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.Version.Input.MustNotBeNullOrEmpty(query.Version);
            await Validate.Version.Input.MustHaveMaximumLenght(query.Version);
            await Validate.Version.ShouldExists(query.Version, _versionRepository);

            await Validate.Style.Input.MustNotBeNullOrEmpty(query.Style);
            await Validate.Style.Input.MustHaveMaximumLenght(query.Style);
            await Validate.Style.ShouldExists(query.Style, _styleRepository);

            return await _exampleLinkRepository.GetExampleLinksByStyleAndVersionAsync(query.Style, query.Version);
        }
    }
}