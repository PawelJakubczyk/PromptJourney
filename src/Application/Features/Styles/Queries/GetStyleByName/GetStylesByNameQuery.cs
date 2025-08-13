using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetStyleByName;

public record GetStylesByNameQuery : IRequest<Result<MidjourneyStyle>>
{
    public required string Name;
}