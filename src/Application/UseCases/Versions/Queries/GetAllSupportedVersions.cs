using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Queries;

public static class GetAllSupportedVersions
{
    public sealed record Query : IQuery<List<string>>
    {
        public static readonly Query Singleton = new();
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
                .MapResult<List<string>>();

            return result;
        }
    }
}
