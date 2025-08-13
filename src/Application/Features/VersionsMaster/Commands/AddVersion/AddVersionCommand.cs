using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Commands.AddVersion;

public record AddVersionCommand : IRequest<Result<MidjourneyVersionsMaster>>
{
    public required MidjourneyVersionsMaster Version { get; init; }
}