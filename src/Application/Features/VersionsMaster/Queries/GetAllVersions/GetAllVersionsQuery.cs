using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.VersionsMaster.Queries.GetAllVersions;

public record GetAllVersionsQuery : IRequest<Result<MidjourneyVersionsMaster>>
{

}