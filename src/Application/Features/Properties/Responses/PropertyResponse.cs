using Domain.Entities;

namespace Application.Features.Properties.Responses;

public sealed record PropertyResponse
(
    string PropertyName,
    string Version,
    List<string>? Parameters,
    string? DefaultValue,
    string? MinValue,
    string? MaxValue,
    string? Description
)
{
    public static PropertyResponse FromDomain(MidjourneyPropertiesBase property) =>
        new(
            property.PropertyName.Value,
            property.Version.Value,
            property.Parameters?.Select(p => p.Value).ToList(),
            property.DefaultValue?.Value,
            property.MinValue?.Value,
            property.MaxValue?.Value,
            property.Description?.Value
        );
}