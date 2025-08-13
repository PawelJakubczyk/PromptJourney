using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Queries.GetAllVersions;

public record GetStyleByTypeQuery : IRequest<Result<List<MidjourneyVersionsMaster>>>
{

}