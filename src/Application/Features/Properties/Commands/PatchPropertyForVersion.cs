using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;

namespace Application.Features.Properties.Commands;

public static class PatchPropertyForVersion
{
    public sealed record Command
    (
        string Version,
        string PropertyName,
        string CharacteristicToUpdate,
        string? NewValue
    ) : ICommand<PropertyDetails>;

    public sealed class Handler(IVersionRepository versionRepository, IPropertiesRepository propertiesRepository)
        : ICommandHandler<Command, PropertyDetails>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<PropertyDetails>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Version.Input.MustNotBeNullOrEmpty(command.Version);
            await Validate.Version.Input.MustHaveMaximumLength(command.Version);
            await Validate.Version.ShouldExists(command.Version, _versionRepository);

            await Validate.Property.Name.Input.MustNotBeNullOrEmpty(command.PropertyName);
            await Validate.Property.Name.Input.MustHaveMaximumLength(command.PropertyName);
            await Validate.Property.ShouldExists(command.Version, command.PropertyName, _propertiesRepository);

            await Validate.Input.MustNotBeNullOrEmpty(command.CharacteristicToUpdate, nameof(command.CharacteristicToUpdate));
            await Validate.Property.ShouldMatchingPropertyName(command.CharacteristicToUpdate);

            return await _propertiesRepository.PatchParameterForVersionAsync
            (
                command.Version,
                command.PropertyName,
                command.CharacteristicToUpdate,
                command.NewValue
            );
        }
    }
}