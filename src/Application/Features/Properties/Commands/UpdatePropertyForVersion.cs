using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Properties.Responses;
using Domain.Entities.MidjourneyProperties;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ErrorsExtensions;

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

    public sealed class Handler(IVersionRepository versionRepository, IPropertiesRepository propertiesRepository)
        : ICommandHandler<Command, PropertyResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
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

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(version.Value, _versionRepository)
                .IfPropertyNotExists(version.Value, propertyName.Value, _propertiesRepository);

            var propertyResult = MidjourneyPropertiesBase.Create
            (
                propertyName.Value,
                version.Value,
                parameters.Select(p => p.Value).ToList(),
                defaultValue?.Value,
                minValue?.Value,
                maxValue?.Value,
                description?.Value
            );

            var domainErrors = propertyResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<PropertyResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _propertiesRepository.UpdateParameterForVersionAsync(propertyResult.Value);

            if (result.IsFailed)
                return Result.Fail<PropertyResponse>(result.Errors);

            var response = PropertyResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}