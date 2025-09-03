using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Properties.Responses;
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
        string Version,
        string PropertyName,
        string CharacteristicToUpdate,
        string? NewValue
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

            List<DomainError> domainErrors = [];

            domainErrors
                .CollectErrors<ModelVersion>(version)
                .CollectErrors<PropertyName>(propertyName);

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionNotExists(version.Value, _versionRepository)
                .IfPropertyNotExists(version.Value, propertyName.Value, _propertiesRepository)
                .IfNoPropertyDetailsFound(command.CharacteristicToUpdate);

            var validationErrors = CreateValidationErrorIfAny<PropertyResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _propertiesRepository.PatchParameterForVersionAsync
            (
                version.Value,
                propertyName.Value,
                command.CharacteristicToUpdate,
                command.NewValue
            );

            if (result.IsFailed)
                return Result.Fail<PropertyResponse>(result.Errors);

            var response = PropertyResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}