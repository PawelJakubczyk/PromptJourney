using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
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
            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .ExecuteAndMapResultIfNoErrors(
                    () => _versionRepository.GetAllVersionsAsync(cancellationToken),
                    versions => versions.Select(VersionResponse.FromDomain).ToList()
                );

            return result;
        }
    }
}