using FluentResults;
using MediatR;

namespace Application.Features.Versions.Commands.DeleteParameterInVersion;

public record DeleteParameterInVersionCommand : IRequest<Result<ParameterDetails>>
{
    public required string Version { get; init; }
    public required string PropertyName { get; init; }
}