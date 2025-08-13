using FluentResults;
using MediatR;

namespace Application.Features.Properties.Commands.UpdatePropertyInVersion;

public record UpdatePropertyInVersionCommand : IRequest<Result<PropertyDetails>>
{
    public required string Version { get; init; }
    public required string PropertyName { get; init; }
    public required string[] Parameters { get; init; }
    public string? DefaultValue { get; init; }
    public string? MinValue { get; init; }
    public string? MaxValue { get; init; }
    public string? Description { get; init; }
}