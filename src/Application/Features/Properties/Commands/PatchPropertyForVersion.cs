using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.Properties.Commands;

public static class PatchPropertyForVersion
{
    public sealed record Command
    (
        ModelVersion Version,
        PropertyName PropertyName,
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
            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(command.Version)
                .CollectErrors<PropertyName>(command.PropertyName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(command.Version, _versionRepository)
                .IfPropertyNotExists(command.Version, command.PropertyName, _propertiesRepository)
                .IfNoPropertyDetailsFound(command.CharacteristicToUpdate);

            var validationErrors = CreateValidationErrorIfAny<PropertyDetails>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

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