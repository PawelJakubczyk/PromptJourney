using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Application.UseCases.Properties.Queries;

public static class CheckPropertyExists
{
    public sealed record Query(string Version, string PropertyName) : IQuery<bool>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
    ) : IQueryHandler<Query, bool>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<bool>> Handle(Query query, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(query.Version);
            var propertyName = PropertyName.Create(query.PropertyName);

            var result = await WorkflowPipeline
                .EmptyAsync()
                .Validate(pipeline => pipeline
                    .CollectErrors(version)
                    .CollectErrors(propertyName))
                .IfVersionNotExists(version.Value, _versionRepository, cancellationToken)
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .CheckPropertyExistsInVersionAsync(version.Value, propertyName.Value, cancellationToken))
                .MapResult<bool>();

            return result;
        }
    }
}