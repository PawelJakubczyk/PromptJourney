using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.ExampleLinks.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class GetExampleLinksByStyle
{
    public sealed record Query(string StyleName) : IQuery<List<ExampleLinkResponse>>;

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
            var styleName = StyleName.Create(query.StyleName);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(styleName)
                .IfStyleNotExists(styleName?.Value!, _styleRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .GetExampleLinksByStyleAsync(styleName.Value, cancellationToken))
                .MapResult<List<MidjourneyStyleExampleLink>, List<ExampleLinkResponse>>
                    (linksList => [.. linksList.Select(ExampleLinkResponse.FromDomain)]);

            return result;
        }
    }
}