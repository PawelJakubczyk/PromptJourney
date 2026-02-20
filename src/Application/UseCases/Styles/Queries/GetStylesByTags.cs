using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Extensions;
using Application.UseCases.Styles.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Queries;

public static class GetStylesByTags
{
    public sealed record Query(List<string>? Tags) : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var tags = query.Tags?.Select(Tag.Create).ToList();

            var result = await WorkflowPipeline
                .EmptyAsync()
                .IfListIsNullOrEmpty(query.Tags)
                .CollectErrors(tags?.ToArray() ?? [])
                .ExecuteIfNoErrors(() => _styleRepository
                    .GetStylesByTagsAsync(tags?.Select(t => t.Value).ToList() ?? [], cancellationToken))
                .MapResult<List<MidjourneyStyle>, List<StyleResponse>>
                    (styleList => [.. styleList.Select(StyleResponse.FromDomain)]);

            return result;
        }
    }
}
