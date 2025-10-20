using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Styles.Queries;

public sealed class GetAllStyleTags
{
    public sealed record Query : IQuery<List<string>>
    {
        public static readonly Query Singletone = new();
    };

    public sealed class Handler(IStyleRepository styleRepository) : IQueryHandler<Query, List<string>>
    {
        private readonly IStyleRepository _styleRepository = styleRepository;

        public async Task<Result<List<string>>> Handle(Query _, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _styleRepository
                    .GetAllStyleTagsAsync(cancellationToken))
                .MapResult<List<Tag>, List<string>>
                    (tagList => [.. tagList.Select(tag => tag.Value)]);

            return result;
        }
    }
}