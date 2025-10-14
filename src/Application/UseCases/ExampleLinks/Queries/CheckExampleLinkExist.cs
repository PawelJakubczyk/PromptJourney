using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class CheckExampleLinkExist
{
    public sealed record Query(string Id) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {

            if (!Guid.TryParse(query.Id, out var id)) {
                return Result.Fail("Invalid GUID format.");
            }

            var result = await WorkflowPipeline
                .EmptyAsync()

                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .CheckExampleLinkExistsAsync(id, cancellationToken))
                .MapResult(() => true);

            return result;
        }
    }
}
