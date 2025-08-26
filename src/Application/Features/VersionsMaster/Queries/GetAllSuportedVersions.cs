using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Features.VersionsMaster.Queries;

public static class GetAllSuportedVersions
{
    public sealed record Query : IQuery<List<string>>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, List<string>>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<string>>> Handle(Query query, CancellationToken cancellationToken)
        {
            await Validate.Version.ShouldHaveAnySuportedVersion(_versionRepository);

            return await _versionRepository.GetAllSuportedVersionsAsync();
        }
    }
}