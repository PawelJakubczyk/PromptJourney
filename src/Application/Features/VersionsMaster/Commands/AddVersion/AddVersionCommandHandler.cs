using Application.Abstractions;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Commands.AddVersion;

public sealed class AddVersionCommandHandler : IRequestHandler<AddVersionCommand, Result<MidjourneyVersionsMaster>>
{
    private readonly IVersionRepository _versionRepository;

    public AddVersionCommandHandler(IVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<Result<MidjourneyVersionsMaster>> Handle(AddVersionCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _versionRepository.AddVersionAsync(command.Version);
            return result;
        }
        catch (Exception ex)
        {
            return Result.Fail<MidjourneyVersionsMaster>($"Error checking version existence: {ex.Message}");
        }
    }
}