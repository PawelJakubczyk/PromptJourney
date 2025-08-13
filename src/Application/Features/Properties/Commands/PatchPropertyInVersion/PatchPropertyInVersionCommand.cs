using FluentResults;
using MediatR;

namespace Application.Features.Properties.Commands.PatchPropertyInVersion;

public record PatchPropertyInVersionCommand : IRequest<Result<PropertyDetails>>
{
    public required string Version { get; init; }
    public required string PropertyName { get; init; }
    public required string PropertyToUpdate { get; init; }
    public string? NewValue { get; init; }
}