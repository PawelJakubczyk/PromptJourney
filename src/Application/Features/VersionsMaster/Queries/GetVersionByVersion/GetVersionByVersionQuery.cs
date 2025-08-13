using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Queries.GetVersionByVersion;

public record GetVersionByVersionQuery : IRequest<Result<MidjourneyVersionsMaster>>
{
    public required string Version { get; init; }
}