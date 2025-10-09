using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

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

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(keyword)
                .ExecuteIfNoErrors(() => _styleRepository
                    .GetStylesByDescriptionKeywordAsync(keyword.Value, cancellationToken))
                .MapResult<List<MidjourneyStyle>, List<StyleResponse>>
                    (styleList => [.. styleList.Select(StyleResponse.FromDomain)]);

            return result;
        }
    }
}