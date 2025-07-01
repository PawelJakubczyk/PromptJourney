using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.CheckVersionExistsInVersionMaster;

public record CheckVersionExistsInVersionMasterQuery : IRequest<Result<bool>>
{
    public required string Version { get; init; }
}