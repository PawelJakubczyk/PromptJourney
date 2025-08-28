using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Features.VersionsMaster.Queries;

public static class GetVersionByVersion
{
    public sealed record Query(string Version) : IQuery<MidjourneyVersion>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, MidjourneyVersion>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyVersion>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.Version.Input.MustNotBeNullOrEmpty(query.Version);
            await Validate.Version.ShouldExists(query.Version, _versionRepository);

            return await _versionRepository.GetMasterVersionByVersionAsync(query.Version);
        }
    }
}