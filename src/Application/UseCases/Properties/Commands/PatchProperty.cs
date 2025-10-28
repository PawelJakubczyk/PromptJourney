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

public static class PatchProperty
{
    public sealed record Command(
        string Version,
        string PropertyName,
        string CharacteristicToUpdate,
        string? NewValue
    ) : ICommand<PropertyCommandResponse>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, PropertyCommandResponse>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<PropertyCommandResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var versionResult = ModelVersion.Create(command.Version);
            var propertyNameResult = PropertyName.Create(command.PropertyName);

            var result = await WorkflowPipeline
                .EmptyAsync()
                    .Congregate(pipeline => pipeline
                        .CollectErrors(versionResult)
                        .CollectErrors(propertyNameResult))
                    .Congregate(pipeline => pipeline
                        .IfVersionNotExists(versionResult.Value, _versionRepository, cancellationToken)
                        .IfPropertyNotExists(propertyNameResult.Value, versionResult.Value, _propertiesRepository, cancellationToken))
                    .ExecuteIfNoErrors(() => _propertiesRepository
                        .PatchPropertyAsync
                        (
                            versionResult.Value,
                            propertyNameResult.Value,
                            command.CharacteristicToUpdate,
                            command.NewValue,
                            cancellationToken
                        ))
                    .MapResult<MidjourneyProperties, PropertyCommandResponse>
                        (property => PropertyCommandResponse.FromDomain(property));

            return result;
        }
    }
}
