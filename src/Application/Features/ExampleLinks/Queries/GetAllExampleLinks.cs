using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.ExampleLinks.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.Features.ExampleLinks.Queries;

public static class GetAllExampleLinks
{
    public sealed record Query : IQuery<List<ExampleLinkResponse>>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, List<ExampleLinkResponse>>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<List<ExampleLinkResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .GetAllExampleLinksAsync(cancellationToken))
                .MapResult<List<MidjourneyStyleExampleLink>, List<ExampleLinkResponse>>
                    (linksList => [.. linksList.Select(ExampleLinkResponse.FromDomain)]);

            return result;
        }
    }
}