using Application.Abstractions;
using Application.Abstractions.IRepository;
using Domain.Entities.MidjourneyVersions;
using FluentResults;

namespace Application.Features.VersionsMaster.Commands;

public static class AddVersion
{
    public sealed record Command(MidjourneyVersions Version) : ICommand<MidjourneyVersions>;

    public sealed class Handler(IVersionRepository versionRepository) : ICommandHandler<Command, MidjourneyVersions>
    {
        private readonly IVersionRepository _versionRepository = versionRepository;

        public async Task<Result<MidjourneyVersions>> Handle(Command command, CancellationToken cancellationToken)
        {
            return await _versionRepository.AddVersionAsync(command.Version);
        }
    }
}