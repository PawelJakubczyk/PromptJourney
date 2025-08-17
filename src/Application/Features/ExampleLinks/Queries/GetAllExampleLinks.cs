using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyStyles;
using FluentResults;

namespace Application.Features.ExampleLinks.Queries;

public static class GetAllExampleLinks
{
    public sealed record Query : IQuery<List<MidjourneyStyleExampleLink>>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository)
        : IQueryHandler<Query, List<MidjourneyStyleExampleLink>>
    {
        private readonly IExampleLinksRepository _exampleLinkRepository = exampleLinkRepository;

        public async Task<Result<List<MidjourneyStyleExampleLink>>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _exampleLinkRepository.GetAllExampleLinksAsync();
        }
    }
}