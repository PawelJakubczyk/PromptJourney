using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.UseCases.Versions.Responses;
using Domain.Entities;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Versions.Queries;

public sealed class GetLatestVersion
{
    public sealed record Query : IQuery<VersionResponse>
    {
        public static readonly Query Singletone = new();
    };

    public sealed class Handler(IVersionRepository versionRepository) : IQueryHandler<Query, VersionResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<VersionResponse>> Handle(Query _, CancellationToken cancellationToken)
        {
            var result = await WorkflowPipeline
                .EmptyAsync()
                .ExecuteIfNoErrors(() => _versionRepository
                    .GetAllVersionsAsync(cancellationToken))
                .MapResult<MidjourneyVersion, VersionResponse>
                    (lastVersion => VersionResponse.FromDomain(lastVersion));

            return result;
        }
    }
}