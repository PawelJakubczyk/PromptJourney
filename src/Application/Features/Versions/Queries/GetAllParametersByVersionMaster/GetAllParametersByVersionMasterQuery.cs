using Domain.Entities.MidjourneyProperties;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.GetMasterVersionByVersion;

public record GetAllParametersByVersionMasterQuery : IRequest<Result<List<MidjourneyPropertiesBase>>>
{
    public required string Version { get; init; }
}