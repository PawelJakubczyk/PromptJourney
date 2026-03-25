using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Properties.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using Utilities.Results;

using Microsoft.Extensions.Caching.Hybrid;
using Utilities.Workflows;

namespace Application.UseCases.Properties.Commands;

public static class UpdateProperty
{
    public sealed record Command
    (
        string Version,
        string PropertyName,
        List<string?>? Parameters = null,
        string? DefaultValue = null,
        string? MinValue = null,
        string? MaxValue = null,
        string? Description = null
    ) : ICommand<PropertyResponse>;

    public sealed class Handler
    (
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository,
        HybridCache cache
    ) : ICommandHandler<Command, PropertyResponse>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly HybridCache _cache = cache;

        public async Task<Result<PropertyResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var versionResult = ModelVersion.Create(command.Version);
            var propertyNameResult = PropertyName.Create(command.PropertyName);
            var parametersResult = command.Parameters is not null ? ParamsCollection.Create(command.Parameters) : Result.Ok(ParamsCollection.None);
            var defaultValueResult = command.DefaultValue is not null ? DefaultValue.Create(command.DefaultValue) : Result.Ok(DefaultValue.None);
            var minValueResult = command.MinValue is not null ? MinValue.Create(command.MinValue) : Result.Ok(MinValue.None);
            var maxValueResult = command.MaxValue is not null ? MaxValue.Create(command.MaxValue) : Result.Ok(MaxValue.None);
            var descriptionResult = command.Description is not null ? Description.Create(command.Description) : Result.Ok(Description.None);

            var propertyResult = MidjourneyProperty.Create
            (
                propertyNameResult.Value,
                versionResult.Value,
                parametersResult.Value,
                defaultValueResult.Value,
                minValueResult.Value,
                maxValueResult.Value,
                descriptionResult.Value
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(propertyResult)
                .CongregateErrors(
                    pipeline => pipeline.IfVersionNotExists(versionResult, _versionRepository, cancellationToken),
                    pipeline => pipeline.IfPropertyNotExists(propertyNameResult, versionResult, _propertiesRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .UpdatePropertyAsync(propertyResult.Value, cancellationToken))
                .MapResult<MidjourneyProperty, PropertyResponse>
                    (property => PropertyResponse.FromDomain(property));

            return result;
        }
    }
}
