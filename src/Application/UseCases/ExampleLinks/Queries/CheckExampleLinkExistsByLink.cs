using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class CheckExampleLinkExistsByLink
{
    public sealed record Query(string Link) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var link = ExampleLink.Create(query.Link);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(link)
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .CheckExampleLinkExistsByLinkAsync(link.Value, cancellationToken))
                .MapResult(() => true);

            return result;
        }
    }
}
