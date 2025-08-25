using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Features.VersionsMaster.Commands;

public static class AddVersion
{
    public sealed record Command
    (
        string Version,
        string Parameter,
        DateTime? ReleaseDate = null,
        string? Description = null
    ) : ICommand<MidjourneyVersions>;

    public sealed class Handler(IVersionRepository versionRepository) : ICommandHandler<Command, MidjourneyVersions>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyVersions>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Version.Input.CannotBeNullOrEmpty(command.Version);
            await Validate.Version.Input.MustHaveMaximumLength(command.Version);
            await Validate.Version.Parameter.Input.CannotBeNullOrEmpty(command.Parameter);
            await Validate.Version.Parameter.Input.MustHaveMaximumLength(command.Parameter);

            return await _versionRepository.AddVersionAsync(command.Version, command.Parameter, command.ReleaseDate, command.Description);
        }
    }
}