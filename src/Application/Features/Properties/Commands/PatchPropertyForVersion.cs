using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;

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

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<PropertyDetails>(error);
            }

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