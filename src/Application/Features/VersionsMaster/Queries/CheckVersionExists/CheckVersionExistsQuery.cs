using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Queries.CheckVersionExists;

public record CheckVersionExistsQuery : IRequest<Result<bool>>
{
    public required string Version { get; init; }
}