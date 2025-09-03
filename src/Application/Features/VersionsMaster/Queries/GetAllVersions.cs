using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.VersionsMaster.Responses;
using FluentResults;

namespace Application.Features.VersionsMaster.Queries;

public static class GetAllVersions
{
    public sealed record Query : IQuery<List<VersionResponse>>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, List<VersionResponse>>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<VersionResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var result = await _versionRepository.GetAllVersionsAsync();

            if (result.IsFailed)
                return Result.Fail<List<VersionResponse>>(result.Errors);

            var responses = result.Value
                .Select(VersionResponse.FromDomain)
                .ToList();

            return Result.Ok(responses);
        }
    }
}