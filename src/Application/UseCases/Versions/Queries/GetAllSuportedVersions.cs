using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Queries;

public static class GetAllSuportedVersions
{
    public sealed record Query : IQuery<List<string>>
    {
        public static readonly Query Singletone = new();
    };

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, List<string>>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<string>>> Handle(Query _, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _versionRepository
                    .GetAllSupportedVersionsAsync(cancellationToken))
                .MapResult<List<ModelVersion>, List<string>>
                    (versionsList => [.. versionsList.Select(v => v.Value)]);

            return result;
        }
    }
}
