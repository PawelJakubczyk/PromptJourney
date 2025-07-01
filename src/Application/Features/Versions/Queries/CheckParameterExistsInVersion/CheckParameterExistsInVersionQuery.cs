using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.CheckParameterExistsInVersion;

public record CheckParameterExistsInVersionQuery : IRequest<Result<bool>>
{
    public required string Version { get; init; }
    public required string PropertyName { get; init; }
}