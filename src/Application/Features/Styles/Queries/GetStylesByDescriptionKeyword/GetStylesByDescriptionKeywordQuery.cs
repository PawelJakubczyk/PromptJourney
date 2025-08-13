using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using MediatR;

namespace Application.Features.Styles.Queries.GetStylesByDescriptionKeyword;

public record GetStylesByDescriptionKeywordQuery : IRequest<Result<List<MidjourneyStyle>>>
{
    public required string DescriptionKeyword;
}