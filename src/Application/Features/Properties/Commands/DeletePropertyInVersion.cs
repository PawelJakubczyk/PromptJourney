using Application.Abstractions;
using Application.Abstractions.IRepository;
using FluentResults;

namespace Application.Features.Properties.Commands;

public static class DeletePropertyInVersion
{
    public sealed record Command(string Version, string PropertyName) : ICommand<PropertyDetails>;

    public sealed class Handler(IVersionRepository versionRepository, IPropertiesRopository propertiesRepository)
        : ICommandHandler<Command, PropertyDetails>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRopository _propertiesRepository = propertiesRepository;

        public async Task<Result<PropertyDetails>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Version.Input.MustNotBeNullOrEmpty(command.Version);
            await Validate.Version.Input.MustHaveMaximumLenght(command.Version);
            await Validate.Version.ShouldExists(command.Version, _versionRepository);

            await Validate.Property.Name.Input.MustNotBeNullOrEmpty(command.PropertyName);
            await Validate.Property.Name.Input.MustHaveMaximumLenght(command.PropertyName);
            await Validate.Property.ShouldExists(command.Version, command.PropertyName, _propertiesRepository);

            return await _propertiesRepository.DeleteParameterInVersionAsync(command.Version, command.PropertyName);
        }
    }
}