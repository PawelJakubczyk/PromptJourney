using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyProperties;
using Domain.Entities.MidjourneyStyles;
using Domain.ValueObjects;
using FluentResults;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Application.Errors.ApplicationErrorMessages;

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

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<PropertyDetails>(error);
            }

            return await _propertiesRepository.UpdateParameterForVersionAsync(propertyResult.Value);
        }
    }
}