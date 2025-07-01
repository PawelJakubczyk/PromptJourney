using FluentResults;
using MediatR;

namespace Application.Features.Versions.Commands.UpdateParameterFromVersion;

public record UpdateParameterFromVersionCommand : IRequest<Result<ParameterDetails>>
{
    public required string Version { get; init; }
    public required string PropertyName { get; init; }
    public required string[] Parameters { get; init; }
    public string? DefaultValue { get; init; }
    public string? MinValue { get; init; }
    public string? MaxValue { get; init; }
    public string? Description { get; init; }
}