using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Properties.Responses;
using Domain.Entities.MidjourneyProperties;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.Properties.Commands;

public static class AddPropertyInVersion
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
            var versionResult = ModelVersion.Create(command.Version);
            var propertyNameResult = PropertyName.Create(command.PropertyName);
            var parametersResult = command.Parameters.Select(p => Param.Create(p)).ToList();
            var defaultValueResult = command.DefaultValue != null ? DefaultValue.Create(command.DefaultValue) : null;
            var minValueResult = command.MinValue != null ? MinValue.Create(command.MinValue) : null;
            var maxValueResult = command.MaxValue != null ? MaxValue.Create(command.MaxValue) : null;
            var descriptionResult = command.Description != null ? Description.Create(command.Description) : null;

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(versionResult.Value, _versionRepository)
                .IfPropertyAlreadyExists(versionResult.Value, propertyNameResult.Value, _propertiesRepository);

            var propertyResult = MidjourneyPropertiesBase.Create
            (
                propertyNameResult,
                versionResult,
                parametersResult,
                defaultValueResult,
                minValueResult,
                maxValueResult,
                descriptionResult
            );

            var domainErrors = propertyResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<PropertyResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _propertiesRepository.AddParameterToVersionAsync(propertyResult.Value);

            if (result.IsFailed)
                return Result.Fail<PropertyResponse>(result.Errors);

            var response = PropertyResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}