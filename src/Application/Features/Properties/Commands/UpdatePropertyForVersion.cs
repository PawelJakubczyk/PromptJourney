using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyProperties;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Properties.Commands;

public static class UpdatePropertyForVersion
{
    public sealed record Command
    (
        ModelVersion Version,
        PropertyName PropertyName,
        List<Param> Parameters,
        DefaultValue? DefaultValue,
        MinValue? MinValue,
        MaxValue? MaxValue,
        Description? Description
    ) : ICommand<PropertyDetails>;

    public sealed class Handler(IVersionRepository versionRepository, IPropertiesRepository propertiesRepository)
        : ICommandHandler<Command, PropertyDetails>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<PropertyDetails>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(command.Version, _versionRepository)
                .IfPropertyNotExists(command.Version, command.PropertyName, _propertiesRepository);

            var propertyResult = MidjourneyPropertiesBase.Create
            (
                command.PropertyName,
                command.Version,
                command.Parameters,
                command.DefaultValue,
                command.MinValue,
                command.MaxValue,
                command.Description
            );

            var domainErrors = propertyResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<PropertyDetails>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _propertiesRepository.UpdateParameterForVersionAsync(propertyResult.Value);
        }
    }
}