using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;
using Application.Extensions;

namespace Application.UseCases.Styles.Queries;

public static class GetStylesByTagsMatchAny
{
    public sealed record Query(List<string>? Tags) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var tags = TagsCollection.Create(query.Tags);

            var tagslist = query.Tags;

            var result = await WorkflowPipeline
                .EmptyAsync()
                .IfListIsNullOrEmpty(query.Tags)
                .CollectErrors(tags)
                .ExecuteIfNoErrors(() => _styleRepository
                    .GetStylesByTagsMatchAnyAsync(tags.Value, cancellationToken))
                .MapResult<List<MidjourneyStyle>, List<StyleResponse>>
                    (styleList => [.. styleList.Select(StyleResponse.FromDomain)]);

            return result;
        }
    }
}
