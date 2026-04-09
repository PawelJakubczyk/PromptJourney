using Domain.Entities;

namespace Application.UseCases.Properties.Responses;

public sealed record PropertyResponse
(
    string PropertyName,
    string Version,
    List<string>? ParamsCollection,
    string? DefaultValue,
    string? MinValue,
    string? MaxValue,
    string? Description
)
{
    public static PropertyResponse FromDomain(MidjourneyProperty property) => new
        (
            property.PropertyName.Value,
            property.Version.Value,
            property.Parameters?.Value.Select(param => param.Value).ToList() ?? null,
            property.DefaultValue?.Value,
            property.MinValue?.Value,
            property.MaxValue?.Value,
            property.Description?.Value
        );
}
