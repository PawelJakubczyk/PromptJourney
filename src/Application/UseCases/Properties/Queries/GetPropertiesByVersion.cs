using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Properties.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Properties.Queries;

public static class GetPropertiesByVersion
{
    public sealed record Query(string Version) : IQuery<List<PropertyResponse>>;

    public sealed class Handler
    (
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
    ) : IQueryHandler<Query, List<PropertyResponse>>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<List<PropertyResponse>>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(version)
                .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .GetAllPropertiesByVersionAsync(version.Value, cancellationToken))
                .MapResult<List<MidjourneyProperties>, List<PropertyResponse>>
                    (propertiesList => [.. propertiesList.Select(PropertyResponse.FromDomain)]);

            return result;
        }
    }
}