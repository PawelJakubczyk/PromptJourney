using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.ExampleLinks.Queries;

public static class CheckAnyExampleLinksExist
{
    public sealed record Query() : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _exampleLinksRepository.CheckAnyExampleLinksExistAsync(cancellationToken))
                .MapResult(_ => true);

            return result;
        }
    }
}