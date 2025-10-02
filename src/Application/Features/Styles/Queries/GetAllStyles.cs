using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.Styles.Responses;
using FluentResults;
using Utilities.Workflows;

namespace Application.Features.Styles.Queries;

public static class GetAllStyles
{
    public sealed record Query : IQuery<List<StyleResponse>>;

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                    .ExecuteIfNoErrors(() => _styleRepository.GetAllStylesAsync(cancellationToken))
                        .MapResult(domainList => domainList.Select(StyleResponse.FromDomain).ToList());

            return result;
        }
    }
}