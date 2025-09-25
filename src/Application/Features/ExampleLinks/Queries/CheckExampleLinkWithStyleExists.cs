using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.ExampleLinks.Queries;

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
                    .ExecuteIfNoErrors(() => _exampleLinksRepository.CheckExampleLinkWithStyleExistsAsync(styleName.Value, cancellationToken))
                        .MapResult(_ => true);

            return result;
        }

    }
}
