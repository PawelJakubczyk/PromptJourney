using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetStylesByTag;

public record GetStylesByTagsQuery : IRequest<Result<List<MidjourneyStyle>>>
{
    public required List<string> Tags;
}