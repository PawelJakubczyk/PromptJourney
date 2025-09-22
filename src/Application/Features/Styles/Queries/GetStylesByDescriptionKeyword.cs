using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Application.Extension;

namespace Application.Features.Styles.Queries;

public static class GetStylesByDescriptionKeyword
{
    public sealed record Query(string DescriptionKeyword) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var keyword = Keyword.Create(query.DescriptionKeyword);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(keyword)
                .ExecuteAndMapResultIfNoErrors(
                    () => _styleRepository.GetStylesByDescriptionKeywordAsync(keyword.Value, cancellationToken),
                    domainList => domainList.Select(StyleResponse.FromDomain).ToList()
                );

            return result;
        }
    }

}