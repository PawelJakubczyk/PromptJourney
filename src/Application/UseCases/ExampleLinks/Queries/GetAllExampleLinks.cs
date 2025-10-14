using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.ExampleLinks.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class GetAllExampleLinks
{
    public sealed record Query : IQuery<ExampleLinkResponse>
    {
        public static readonly Query Simgletone = new();
    };

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, ExampleLinkResponse>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<ExampleLinkResponse>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .GetAllExampleLinksAsync(cancellationToken))
                .MapResult<MidjourneyStyleExampleLink, ExampleLinkResponse>
                    (link => ExampleLinkResponse.FromDomain(link));

            return result;
        }
    }
}
