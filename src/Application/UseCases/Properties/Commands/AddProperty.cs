using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.UseCases.Properties.Responses;
using Domain.Entities;
using Utilities.Results;
using Domain.ValueObjects;

using Utilities.Workflows;

namespace Application.UseCases.Properties.Commands;

public static class AddProperty
{
    public sealed record Command
    (
        string Version,
        string PropertyName,
        List<string?> Parameters,
        string? DefaultValue = null,
        string? MinValue = null,
        string? MaxValue = null,
        string? Description = null
    ) : ICommand<PropertyCommandResponse>;

    public sealed class Handler
    (
        IVersionRepository versionRepository,
        IPropertiesRepository propertiesRepository
    ) : ICommandHandler<Command, PropertyCommandResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<PropertyCommandResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var versionResult = ModelVersion.Create(command.Version);
            var propertyNameResult = PropertyName.Create(command.PropertyName);
            var parametersResult = ParamsCollection.Create(command.Parameters);
            var defaultValueResult = command.DefaultValue is not null ? DefaultValue.Create(command.DefaultValue) : DefaultValue.None;
            var minValueResult = command.MinValue is not null ? MinValue.Create(command.MinValue) : MinValue.None;
            var maxValueResult = command.MaxValue is not null ? MaxValue.Create(command.MaxValue) : MaxValue.None;
            var descriptionResult = command.Description is not null ? Description.Create(command.Description) : Description.None;

            var property = MidjourneyProperty.Create
            (
                propertyNameResult,
                versionResult,
                parametersResult,
                defaultValueResult!,
                minValueResult!,
                maxValueResult!,
                descriptionResult!
            );

            var result = await WorkflowPipeline
                .EmptyAsync()
                .CollectErrors(property)
                .CongregateErrors(
                    pipeline => pipeline.IfVersionNotExists(versionResult.Value, _versionRepository, cancellationToken),
                    pipeline => pipeline.IfPropertyAlreadyExists(propertyNameResult.Value, versionResult.Value, _propertiesRepository, cancellationToken))
                .ExecuteIfNoErrors(() => _propertiesRepository
                    .AddPropertyAsync(property.Value, cancellationToken))
                .MapResult<MidjourneyProperty, PropertyCommandResponse>
                    (property => PropertyCommandResponse.FromDomain(property));

            return result;
        }
    }
}
