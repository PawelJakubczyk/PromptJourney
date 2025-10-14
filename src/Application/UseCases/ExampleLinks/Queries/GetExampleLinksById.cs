using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.ExampleLinks.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class GetExampleLinksById
{
    public sealed record Query(string Id) : IQuery<List<ExampleLinkResponse>>;

    public sealed class Handler
    (
        IExampleLinksRepository exampleLinkRepository,
        IStyleRepository styleRepository
    ) : IQueryHandler<Query, List<ExampleLinkResponse>>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinkRepository;
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<ExampleLinkResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var LinkId = MidjourneyStyleExampleLink.ParseLinkId(query.Id);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(LinkId)
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .GetExampleLinksByIdAsync(LinkId.Value, cancellationToken))
                .MapResult<List<MidjourneyStyleExampleLink>, List<ExampleLinkResponse>>
                    (linksList => [.. linksList.Select(ExampleLinkResponse.FromDomain)]);

            return result;
        }
    }
}
