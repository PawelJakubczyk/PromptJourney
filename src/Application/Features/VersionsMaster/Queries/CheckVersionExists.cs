using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;

namespace Application.Features.VersionsMaster.Queries;

public static class CheckVersionExists
{
    public sealed record Query(string Version) : IQuery<bool>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, bool>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _versionRepository.CheckVersionExistsInVersionsAsync(query.Version);
        }
    }
}