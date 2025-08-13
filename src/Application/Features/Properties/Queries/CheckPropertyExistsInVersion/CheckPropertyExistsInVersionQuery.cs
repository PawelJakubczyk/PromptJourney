using FluentResults;
using MediatR;

namespace Application.Features.Properties.Queries.CheckPropertyExistsInVersion;

public record CheckPropertyExistsInVersionQuery : IRequest<Result<bool>>
{
    public required string Version { get; init; }
    public required string PropertyName { get; init; }
}`