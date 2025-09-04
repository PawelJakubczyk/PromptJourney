using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;
using Application.Features.Common.Responses;

namespace Application.Features.Properties.Commands;

public static class DeletePropertyInVersion
{
    public sealed record Command(string Version, string PropertyName) : ICommand<DeleteResponse>;

    public sealed class Handler(IVersionRepository versionRepository, IPropertiesRepository propertiesRepository)
        : ICommandHandler<Command, DeleteResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;
        private readonly IPropertiesRepository _propertiesRepository = propertiesRepository;

        public async Task<Result<DeleteResponse>> Handle(Command command, CancellationToken cancellationToken)
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
                .IfPropertyNotExists(version.Value, propertyName.Value, _propertiesRepository);

            var validationErrors = CreateValidationErrorIfAny<DeleteResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _propertiesRepository.DeleteParameterInVersionAsync(version.Value, propertyName.Value);

            if (result.IsFailed)
                return Result.Fail<DeleteResponse>(result.Errors);

            var response = DeleteResponse.Success($"Property '{propertyName.Value.Value}' was successfully deleted from version '{version.Value.Value}'.");

            return Result.Ok(response);
        }
    }
}