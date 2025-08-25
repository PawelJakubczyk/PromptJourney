using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;

namespace Application.Features.Properties.Commands;

public static class AddPropertyInVersion
{
    public sealed record Command
    (
        string Version,
        string PropertyName,
        string[] Parameters,
        string? DefaultValue,
        string? MinValue,
        string? MaxValue,
        string? Description
    ) : ICommand<PropertyDetails>;

    public sealed class Handler(IVersionRepository versionRepository, IPropertiesRopository propertiesRepository)
        : ICommandHandler<Command, PropertyDetails>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRopository _propertiesRepository = propertiesRepository;

        public async Task<Result<PropertyDetails>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Version.Input.MustNotBeNullOrEmpty(command.Version);
            await Validate.Version.Input.MustHaveMaximumLenght(command.Version);

            await Validate.Property.Name.Input.MustNotBeNullOrEmpty(command.PropertyName);
            await Validate.Property.Name.Input.MustHaveMaximumLenght(command.PropertyName);

            await Validate.Property.Parameters.Input.MustNotBeNull(command.Parameters);
            await Validate.Property.Parameters.Input.MustHaveAtLeastOneElement(command.Parameters);
            await Validate.Property.Parameters.Input.MustNotHaveMoreThanXElements(command.Parameters, 5);
            await Validate.Property.Parameters.Input.MustNotHaveElementsThatAreNullOrEmpty(command.Parameters);

            await Validate.Version.ShouldExists(command.Version, _versionRepository);
            await Validate.Property.ShouldNotExists(command.Version, command.PropertyName, _propertiesRepository);

            return await _propertiesRepository.AddParameterToVersionAsync
            (
                command.Version,
                command.PropertyName,
                command.Parameters,
                command.DefaultValue,
                command.MinValue,
                command.MaxValue,
                command.Description
            );
        }
    }
}