using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Extensions;
using Domain.Entities.MidjourneyVersions;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;

namespace Application.Features.VersionsMaster.Commands;

public static class AddVersion
{
    public sealed record Command
    (
        ModelVersion Version,
        Param Parameter,
        DateTime? ReleaseDate = null,
        Description? Description = null
    ) : ICommand<MidjourneyVersion>;

    public sealed class Handler(IVersionRepository versionRepository) : ICommandHandler<Command, MidjourneyVersion>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyVersion>> Handle(Command command, CancellationToken cancellationToken)
        {
            List<ApplicationError> applicationErrors = [];

            applicationErrors
                .IfVersionAlreadyExists(command.Version, _versionRepository);

            var VersionResult = MidjourneyVersion.Create
            (
                command.Version,
                command.Parameter,
                command.ReleaseDate,
                command.Description
            );

            var domainErrors = VersionResult.Errors;

            if (applicationErrors.Count != 0 || domainErrors.Count != 0)
            {
                var error = new Error("Validation failed")
                    .WithMetadata("Application Errors", applicationErrors)
                    .WithMetadata("Domain Errors", domainErrors);

                return Result.Fail<MidjourneyVersion>(error);
            }

            return await _versionRepository.AddVersionAsync(VersionResult.Value);
        }
    }
}