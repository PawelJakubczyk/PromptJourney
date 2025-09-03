using Domain.Entities.MidjourneyStyles;

namespace Application.Features.Styles.Responses;

public sealed record StyleResponse
(
    string Name,
    string Type,
    string? Description,
    List<string>? Tags
)
{
    public static StyleResponse FromDomain(MidjourneyStyle style) =>
    new(
        style.Name.Value, 
        style.Type.Value, 
        style.Description?.Value,
        style.Tags?.Select(t => t.Value).ToList()
    );
}