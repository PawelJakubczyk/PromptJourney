using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Styles.Responses;
using Domain.ValueObjects;
using FluentResults;
using Application.Extension;

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

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .IfListIsNullOrEmpty(query.Tags)
                .CollectErrors<List<Result<Tag>>>(tags!)
                .ExecuteAndMapResultIfNoErrors(
                    () => _styleRepository.GetStylesByTagsAsync(tags?.Select(t => t.Value).ToList() ?? [], cancellationToken),
                    domainList => domainList.Select(StyleResponse.FromDomain).ToList()
                );

            return result;
        }
    }

}