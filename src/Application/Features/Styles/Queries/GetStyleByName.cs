using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Application.Extension;

namespace Application.Features.Styles.Queries;

public static class GetStyleByName
{
    public sealed record Query(string StyleName) : IQuery<StyleResponse>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, StyleResponse>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<StyleResponse>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleName = StyleName.Create(query.StyleName);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleName)
                .IfStyleNotExists(styleName.Value, _styleRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors(
                    () => _styleRepository.GetStyleByNameAsync(styleName.Value, cancellationToken),
                    StyleResponse.FromDomain
                );

            return result;
        }
    }

}