using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Styles.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Queries;

public static class GetAllStyles
{
    public sealed record Query : IQuery<List<StyleResponse>>
    {
        public static readonly Query Singletone = new();
    };

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<StyleResponse>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<StyleResponse>>> Handle(Query _, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _styleRepository
                    .GetAllStylesAsync(cancellationToken))
                .MapResult<List<MidjourneyStyle>, List<StyleResponse>>
                    (styleList => [.. styleList.Select(StyleResponse.FromDomain)]);

            return result;
        }
    }
}
