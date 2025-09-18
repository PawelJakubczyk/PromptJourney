using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Features.VersionsMaster.Responses;
using Domain.Entities;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorsExtensions;

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

            var versionResult = MidjourneyVersion.Create
            (
                version,
                parameter,
                command.ReleaseDate,
                description!
            );

            var domainErrors = versionResult.Errors;

            var validationErrors = CreateValidationErrorIfAny<VersionResponse>
            (
                (nameof(domainErrors), domainErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var addResult = await _versionRepository.AddVersionAsync(versionResult.Value);

            var persistenceErrors = addResult.Errors;

            validationErrors = CreateValidationErrorIfAny<VersionResponse>
            (
                (nameof(persistenceErrors), persistenceErrors)
            );
            if (validationErrors is not null) return validationErrors;

            var response = VersionResponse.FromDomain(addResult.Value);

            return Result.Ok(response);
        }
    }
}