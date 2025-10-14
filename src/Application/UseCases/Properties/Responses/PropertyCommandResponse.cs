using Domain.Entities;

namespace Application.UseCases.Properties.Responses;

public sealed record PropertyCommandResponse
(
    string PropertyName,
    string Version
)
{
    public static PropertyCommandResponse FromDomain(MidjourneyProperties property) => new
    (
        property.PropertyName.Value,
        property.Version.Value
    );
}
