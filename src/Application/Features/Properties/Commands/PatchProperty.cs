using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Application.Features.Properties.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.Extensions.Caching.Hybrid;
using Utilities.Workflows;

namespace Application.Features.Properties.Commands;

public static class PatchProperty
{
    public sealed record Command(
        string Version,
        string PropertyName,
        string CharacteristicToUpdate,
        string? NewValue
    ) : ICommand<PropertyResponse>;

    public sealed class Handler(
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

            var result = await WorkflowPipeline
                .EmptyAsync()
                    .Validate(pipeline => pipeline
                        .CollectErrors(versionResult)
                        .CollectErrors(propertyNameResult))
                    .Validate(pipeline => pipeline
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
                    .MapResult<MidjourneyProperties, PropertyResponse>
                        (property => PropertyResponse.FromDomain(property));

            return result;
        }
    }
}