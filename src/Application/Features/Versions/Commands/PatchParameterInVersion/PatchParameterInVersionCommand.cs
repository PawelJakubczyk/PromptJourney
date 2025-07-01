using FluentResults;
using MediatR;

namespace Application.Features.Versions.Commands.PatchParameterInVersion;

public record PatchParameterInVersionCommand : IRequest<Result<ParameterDetails>>
{
    public required string Version { get; init; }
    public required string PropertyName { get; init; }
    public required string PropertyToUpdate { get; init; }
    public string? NewValue { get; init; }
}