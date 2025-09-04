using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.ValueObjects;
using FluentResults;

namespace Application.Features.VersionsMaster.Queries;

public static class GetAllSuportedVersions
{
    public sealed record Query : IQuery<List<ModelVersion>>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, List<ModelVersion>>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<ModelVersion>>> Handle(Query query, CancellationToken cancellationToken)
        {
            return await _versionRepository.GetAllSuportedVersionsAsync();
        }
    }
}