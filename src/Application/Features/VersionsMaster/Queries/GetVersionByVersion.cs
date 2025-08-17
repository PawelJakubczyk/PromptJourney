using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Features.VersionsMaster.Queries;

public static class GetVersionByVersion
{
    public sealed record Query(string Version) : IQuery<MidjourneyVersions>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, MidjourneyVersions>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyVersions>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _versionRepository.GetMasterVersionByVersionAsync(query.Version);
        }
    }
}