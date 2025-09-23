using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extension;
using Application.Features.Properties.Responses;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Application.Features.Properties.Commands;

public static class PatchPropertyForVersion
{
    public sealed record Command(
        string Version,
        string PropertyName,
        string CharacteristicToUpdate,
        string? NewValue
    ) : ICommand<PropertyResponse>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, PropertyResponse>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<PropertyResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);
            var propertyName = PropertyName.Create(command.PropertyName);

            var result = await ValidationPipeline
                .EmptyAsync()
                    .BeginValidationBlock()
                        .CollectErrors(version)
                        .CollectErrors(propertyName)
                    .EndValidationBlock()
                    .IfNoErrors()
                        .Executes(() => _propertiesRepository.PatchParameterForVersionAsync
                        (
                            version.Value,
                            propertyName.Value,
                            command.CharacteristicToUpdate,
                            command.NewValue,
                            cancellationToken
                        ))
                        .MapResult(PropertyResponse.FromDomain);

            return result;
        }

    }
}