using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.Versions.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.Features.Versions.Queries;

public static class GetAllVersions
{
    public sealed record Query : IQuery<List<VersionResponse>>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, List<VersionResponse>>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<VersionResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _versionRepository
                    .GetAllVersionsAsync(cancellationToken))
                .MapResult<List<MidjourneyVersion>, List<VersionResponse>>
                    (versions => [.. versions.Select(VersionResponse.FromDomain)]);

            return result;
        }
    }
}