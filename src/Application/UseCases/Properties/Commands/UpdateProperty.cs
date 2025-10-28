using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Properties.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.Extensions.Caching.Hybrid;
using Utilities.Workflows;

namespace Application.UseCases.Properties.Commands;

public static class UpdateProperty
{
    public sealed record Command
    (
        string Version,
        string PropertyName,
        List<string> Parameters,
        string? DefaultValue,
        string? MinValue,
        string? MaxValue,
        string? Description
    ) : ICommand<PropertyCommandResponse>;

    public sealed class Handler
    (
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository,
        HybridCache cache
    ) : ICommandHandler<Command, PropertyCommandResponse>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly HybridCache _cache = cache;

        public async Task<Result<PropertyCommandResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var versionResult = ModelVersion.Create(command.Version);
            var propertyNameResult = PropertyName.Create(command.PropertyName);
            var parametersResult = command.Parameters.Select(Param.Create).ToList();
            var defaultValueResult = command.DefaultValue is not null ? DefaultValue.Create(command.DefaultValue) : null;
            var minValueResult = command.MinValue is not null ? MinValue.Create(command.MinValue) : null;
            var maxValueResult = command.MaxValue is not null ? MaxValue.Create(command.MaxValue) : null;
            var descriptionResult = command.Description is not null ? Description.Create(command.Description) : null;

            var propertyResult = MidjourneyProperties.Create
            (
                propertyNameResult.Value,
                versionResult.Value,
                parametersResult,
                defaultValueResult?.Value,
                minValueResult?.Value,
                maxValueResult?.Value,
                descriptionResult?.Value
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(propertyResult)
                .Congregate(pipeline => pipeline
                    .IfVersionNotExists(versionResult.Value, _versionRepository, cancellationToken)
                    .IfPropertyNotExists(propertyNameResult.Value, versionResult.Value, _propertiesRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .UpdatePropertyAsync(propertyResult.Value, cancellationToken))
                .MapResult<MidjourneyProperties, PropertyCommandResponse>
                    (property => PropertyCommandResponse.FromDomain(property));

            return result;
        }
    }
}
