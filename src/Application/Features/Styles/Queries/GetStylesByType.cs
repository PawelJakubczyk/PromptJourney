using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.Styles.Queries;

public static class GetStylesByType
{
    public sealed record Query(string StyleType) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var styleType = StyleType.Create(query.StyleType);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(styleType)
                .ExecuteAndMapResultIfNoErrors(
                    () => _styleRepository.GetStylesByTypeAsync(styleType.Value, cancellationToken),
                    domainList => domainList.Select(StyleResponse.FromDomain).ToList()
                );

            return result;
        }
    }
}