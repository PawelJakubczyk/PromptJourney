using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;
using Domain.ValueObjects;
using Application.Extension;

namespace Application.Features.VersionsMaster.Queries;

public static class CheckVersionExists
{
    public sealed record Query(string Version) : IQuery<bool>;

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, bool>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);

            var result = await ErrorFactory
                .EmptyErrorsAsync()
                .CollectErrors(version)
                .ExecuteAndMapResultIfNoErrors(
                    () => _versionRepository.CheckVersionExistsInVersionsAsync(version.Value, cancellationToken),
                    value => value
                );

            return result;
        }
    }

}