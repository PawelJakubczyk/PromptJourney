using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Features.VersionsMaster.Queries;

public static class GetAllVersions
{
    public sealed record Query : IQuery<List<MidjourneyVersion>>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, List<MidjourneyVersion>>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<MidjourneyVersion>>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _versionRepository.GetAllVersionsAsync();
        }
    }
}