using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.ExampleLinks.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class GetExampleLinksById
{
    public sealed record Query(string Id) : IQuery<ExampleLinkResponse>;

    public sealed class Handler(IExampleLinksRepository exampleLinkRepository) : IQueryHandler<Query, ExampleLinkResponse>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinkRepository;

        public async Task<Result<ExampleLinkResponse>> Handle(Query query, CancellationToken cancellationToken)
        {
            var LinkId = MidjourneyStyleExampleLink.ParseLinkId(query.Id);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(LinkId)
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .GetExampleLinkByIdAsync(LinkId.Value, cancellationToken))
                .MapResult<MidjourneyStyleExampleLink, ExampleLinkResponse>
                    (link => ExampleLinkResponse.FromDomain(link));

            return result;
        }
    }
}
