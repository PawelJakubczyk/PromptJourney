using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class CheckExampleLinkWithIdExists
{
    public sealed record Query(string Id) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {

            var linkId = MidjourneyStyleExampleLink.ParseLinkId(query.Id);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(linkId)
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .CheckExampleLinkExistsByIdAsync(linkId.Value, cancellationToken))
                .MapResult(() => true);

            return result;
        }
    }
}
