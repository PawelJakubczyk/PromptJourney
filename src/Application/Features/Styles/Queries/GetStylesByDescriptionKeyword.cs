using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

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

            var result = await ValidationPipeline
                .EmptyAsync()
                .CollectErrors(keyword)
                .IfNoErrors()
                    .Executes(() => _styleRepository.GetStylesByDescriptionKeywordAsync(keyword.Value, cancellationToken))
                        .MapResult(domainList => domainList.Select(StyleResponse.FromDomain).ToList());

            return result;
        }
    }

}