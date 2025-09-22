using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Application.Extension;

namespace Application.Features.Styles.Queries;

public class CheckTagExistInStyle
{
    public sealed record Query(string StyleName, string Tag) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);
            var tag = Tag.Create(query.Tag);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleName)
                .CollectErrors(tag)
                .IfTagNotExist(styleName.Value, tag.Value, _styleRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors(
                    () => _styleRepository.CheckTagExistsInStyleAsync(styleName.Value, tag.Value, cancellationToken),
                    value => value
                );

            return result;
        }
    }
}
