using FluentResults;
using MediatR;

namespace Application.Features.Properties.Commands.DeletePropertyInVersion;

public record DeletePropertyInVersionCommand : IRequest<Result<PropertyDetails>>
{
    public required string Version { get; init; }
    public required string PropertyName { get; init; }
}