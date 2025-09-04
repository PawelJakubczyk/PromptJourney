using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Application.Features.VersionsMaster.Responses;
using Domain.Entities.MidjourneyVersions;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ErrorsExtensions;

namespace Application.Features.VersionsMaster.Commands;

public static class AddVersion
{
    public sealed record Command
    (
        string Version,
        string Parameter,
        DateTime? ReleaseDate = null,
        string? Description = null
    ) : ICommand<VersionResponse>;

    public sealed class Handler(IVersionRepository versionRepository) : ICommandHandler<Command, VersionResponse>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<VersionResponse>> Handle(Command command, CancellationToken cancellationToken)
        {
            var version = ModelVersion.Create(command.Version);
            var parameter = Param.Create(command.Parameter);
            var description = command.Description != null ? Description.Create(command.Description) : null;

            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionAlreadyExists(version.Value, _versionRepository);

            var versionResult = MidjourneyVersion.Create
            (
                version.Value,
                parameter.Value,
                command.ReleaseDate,
                description?.Value
            );

            var domainErrors = versionResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<VersionResponse>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            var result = await _versionRepository.AddVersionAsync(versionResult.Value);

            if (result.IsFailed)
                return Result.Fail<VersionResponse>(result.Errors);

            var response = VersionResponse.FromDomain(result.Value);

            return Result.Ok(response);
        }
    }
}