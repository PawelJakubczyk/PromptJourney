using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Properties.Responses;
using Domain.Entities;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.Properties.Commands;

public static class UpdatePropertyForVersion
{
    public sealed record Command
    (
        string Version,
        string PropertyName,
        List<string> Parameters,
        string? DefaultValue,
        string? MinValue,
        string? MaxValue,
        string? Description
    ) : ICommand<PropertyResponse>;

    public sealed class Handler
    (
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, PropertyResponse>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<PropertyResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);
            var propertyName = PropertyName.Create(command.PropertyName);
            var parameters = command.Parameters.Select(p => Param.Create(p)).ToList();
            var defaultValue = command.DefaultValue != null ? DefaultValue.Create(command.DefaultValue) : null;
            var minValue = command.MinValue != null ? MinValue.Create(command.MinValue) : null;
            var maxValue = command.MaxValue != null ? MaxValue.Create(command.MaxValue) : null;
            var description = command.Description != null ? Description.Create(command.Description) : null;

            var propertyResult = MidjourneyPropertiesBase.Create
            (
                propertyName.Value,
                version.Value,
                parameters,
                defaultValue?.Value,
                minValue?.Value,
                maxValue?.Value,
                description?.Value
            );

            var domainErrors = propertyResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<PropertyResponse>
            (
                (nameof(domainErrors), domainErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var updateResult = await _propertiesRepository.UpdateParameterForVersionAsync(propertyResult.Value);
            var persistenceErrors = updateResult.Errors;

            validationErrors = CreateValidationErrorIfAny<PropertyResponse>
            (
                (nameof(persistenceErrors), persistenceErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var response = PropertyResponse.FromDomain(updateResult.Value);

            return Result.Ok(response);
        }
    }
}