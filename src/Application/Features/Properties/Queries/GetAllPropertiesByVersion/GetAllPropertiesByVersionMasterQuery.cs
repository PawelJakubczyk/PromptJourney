using Domain.Entities.MidjourneyProperties;
using FluentResults;
using MediatR;

namespace Application.Features.Properties.Queries.GetAllPropertiesByVersion;

public record GetAllPropertiesByVersionMasterQuery : IRequest<Result<List<MidjourneyPropertiesBase>>>
{
    public required string Version { get; init; }
}