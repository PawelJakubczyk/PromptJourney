using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.VersionsMaster.Responses;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.VersionsMaster.Queries;

public static class GetVersionByVersion
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
                .ExecuteIfNoErrors(() => _versionRepository.GetMasterVersionByVersionAsync(version.Value, cancellationToken))
                .MapResult(VersionResponse.FromDomain);

            return result;
        }
    }

}