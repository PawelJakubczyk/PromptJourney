using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Queries;

public class CheckTagExistsInStyle
{
    public sealed record Query(string StyleName, string Tag) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);
            var tag = Tag.Create(query.Tag);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .Congregate(pipeline => pipeline
                    .CollectErrors(styleName)
                    .CollectErrors(tag))
                .ExecuteIfNoErrors(() => _styleRepository
                    .CheckTagExistsInStyleAsync(styleName.Value, tag.Value, cancellationToken))
                .MapResult<bool>();

            return result;
        }
    }
}