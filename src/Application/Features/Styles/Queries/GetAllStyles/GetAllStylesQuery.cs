using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetAllStyles;

public record GetAllStylesQuery : IRequest<Result<List<MidjourneyStyle>>>
{

}