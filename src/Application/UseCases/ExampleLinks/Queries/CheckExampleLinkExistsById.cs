using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class CheckExampleLinkExistsById
{
    public sealed record Query(string? Id) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var linkId = LinkID.Create(query.Id);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(linkId)
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .CheckExampleLinkExistsByIdAsync(linkId.Value, cancellationToken))
                .MapResult<bool>();

            return result;
        }
    }
}
