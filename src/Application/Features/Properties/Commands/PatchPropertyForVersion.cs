using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.Properties.Responses;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

namespace Application.Features.Properties.Commands;

public static class PatchPropertyForVersion
{
    public sealed record Command(
        string Version,
        string PropertyName,
        string CharacteristicToUpdate,
        string? NewValue
    ) : ICommand<PropertyResponse>;

    public sealed class Handler(
        IPropertiesRepository propertiesRepository,
        IVersionRepository versionRepository
    ) : ICommandHandler<Command, PropertyResponse>
    {
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<PropertyResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);
            var propertyName = PropertyName.Create(command.PropertyName);

            List<DomainError> domainErrors = [];
            domainErrors
                .CollectErrors(version)
                .CollectErrors(propertyName);

            var validationErrors = CreateValidationErrorIfAny<PropertyResponse>
            (
                (nameof(domainErrors), domainErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var patchResult = await _propertiesRepository.PatchParameterForVersionAsync
            (
                version.Value,
                propertyName.Value,
                command.CharacteristicToUpdate,
                command.NewValue
            );
            var persistenceErrors = patchResult.Errors;

            validationErrors = CreateValidationErrorIfAny<PropertyResponse>(
                (nameof(persistenceErrors), persistenceErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var response = PropertyResponse.FromDomain(patchResult.Value);
            return Result.Ok(response);
        }
    }
}