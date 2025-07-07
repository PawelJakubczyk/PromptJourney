using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Versions.Queries.GetMasterVersionByVersion;

public record GetAllParametersByVersionMasterQuery : IRequest<Result<List<MidjourneyVersionsBase>>>
{
    public required string Version { get; init; }
}