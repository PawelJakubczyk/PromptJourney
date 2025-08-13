using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetStyleByType;

public record GetStylesByTypeQuery : IRequest<Result<List<MidjourneyStyle>>>
{
    public required string Type;
}