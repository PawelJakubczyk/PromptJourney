using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.Features.Styles.Queries;

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
                .CollectErrors(tags!)
                .ExecuteIfNoErrors(() => _styleRepository.GetStylesByTagsAsync(tags?.Select(t => t.Value).ToList() ?? [], cancellationToken))
                .MapResult(domainList => domainList.Select(StyleResponse.FromDomain).ToList());

            return result;
        }
    }

}