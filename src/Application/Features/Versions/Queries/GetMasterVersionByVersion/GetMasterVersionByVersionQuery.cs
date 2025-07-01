using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.GetMasterVersionByVersion;

public record GetMasterVersionByVersionQuery : IRequest<Result<MidjourneyVersionsMaster>>
{
    public required string Version { get; init; }
}