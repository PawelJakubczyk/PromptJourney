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
    ) : ICommand<MidjourneyVersion>;

    public sealed class Handler(IVersionRepository versionRepository) : ICommandHandler<Command, MidjourneyVersion>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyVersion>> Handle(Command command, CancellationToken cancellationToken)
        {
            await Validate.Version.Input.MustNotBeNullOrEmpty(command.Version);
            await Validate.Version.Input.MustHaveMaximumLength(command.Version);
            await Validate.Version.Parameter.Input.MustNotBeNullOrEmpty(command.Parameter);
            await Validate.Version.Parameter.Input.MustHaveMaximumLength(command.Parameter);

            return await _versionRepository.AddVersionAsync(command.Version, command.Parameter, command.ReleaseDate, command.Description);
        }
    }
}