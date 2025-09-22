using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Application.Extension;

namespace Application.Features.Styles.Queries;

public static class CheckStyleExist
{
    public sealed record Query(string StyleName) : IQuery<bool>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, bool>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleName)
                .ExecuteAndMapResultIfNoErrors(
                    () => _styleRepository.CheckStyleExistsAsync(styleName.Value, cancellationToken),
                    value => value
                );

            return result;
        }

    }
}