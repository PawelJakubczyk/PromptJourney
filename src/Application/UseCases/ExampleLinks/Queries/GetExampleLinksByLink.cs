using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.ExampleLinks.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class GetExampleLinksByLink
{
    public sealed record Query(string Link) : IQuery<List<ExampleLinkResponse>>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, List<ExampleLinkResponse>>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<List<ExampleLinkResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(query.Link);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(link)
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .GetExampleLinkByLinkAsync(link.Value, cancellationToken))
                .MapResult<List<MidjourneyStyleExampleLink>, List<ExampleLinkResponse>>
                    (links => [.. links.Select(ExampleLinkResponse.FromDomain)]);

            return result;
        }
    }
}