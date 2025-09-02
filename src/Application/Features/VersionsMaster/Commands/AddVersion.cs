using Application.Abstractions;
using Application.Abstractions.IRepository;
using Application.Errors;
using Domain.Entities.MidjourneyVersions;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Application.Errors.ErrorsExtensions;

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

            var validationErrors = CreateValidationErrorIfAny<MidjourneyVersion>(applicationErrors, domainErrors);
            if (validationErrors is not null) return validationErrors;

            return await _versionRepository.AddVersionAsync(VersionResult.Value);
        }
    }
}