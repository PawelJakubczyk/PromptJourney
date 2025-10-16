using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.ExampleLinks.Queries;

public static class CheckExampleLinkWithStyleExists
{
    public sealed record Query(string StyleName) : IQuery<bool>;

    public sealed class Handler(IExampleLinksRepository exampleLinksRepository)
        : IQueryHandler<Query, bool>
    {
        private readonly IExampleLinksRepository _exampleLinksRepository = exampleLinksRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(styleName)
                .ExecuteIfNoErrors(() => _exampleLinksRepository
                    .CheckExampleLinkExistsByStyleAsync(styleName.Value, cancellationToken))
                .MapResult(() => true);

            return result;
        }
    }
}