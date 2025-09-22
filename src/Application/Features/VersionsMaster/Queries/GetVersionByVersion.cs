using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.VersionsMaster.Responses;
using Domain.ValueObjects;
using FluentResults;

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

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(version)
                .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                .ExecuteAndMapResultIfNoErrors(
                    () => _versionRepository.GetMasterVersionByVersionAsync(version.Value, cancellationToken),
                    VersionResponse.FromDomain
                );

            return result;
        }
    }

}