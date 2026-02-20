using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Versions.Responses;
using Domain.Entities;
using Utilities.Results;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Queries;

public sealed class GetLatestVersion
{
    public sealed record Query : IQuery<VersionResponse>
    {
        public static readonly Query Singleton = new();
    }

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, VersionResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<VersionResponse>> Handle(Query _, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _versionRepository
                    .GetLatestVersionAsync(cancellationToken))
                .MapResult<MidjourneyVersion, VersionResponse>(latestVersion => VersionResponse.FromDomain(latestVersion));

            return result;
        }
    }
}