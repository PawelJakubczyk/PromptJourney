using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Versions.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Queries;

public static class GetVersion
{
    public sealed record Query(string Version) : IQuery<VersionResponse>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, VersionResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<VersionResponse>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(version)
                .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _versionRepository
                    .GetVersionAsync(version.Value, cancellationToken))
                .MapResult<MidjourneyVersion, VersionResponse>
                    (version => VersionResponse.FromDomain(version));

            return result;
        }
    }
}