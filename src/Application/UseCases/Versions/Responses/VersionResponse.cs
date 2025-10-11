using Domain.Entities;

namespace Application.UseCases.Versions.Responses;

public sealed record VersionResponse
(
    string Version,
    string Parameter,
    DateTime? ReleaseDate,
    string? Description
)
{
    public static VersionResponse FromDomain(MidjourneyVersion version) =>
        new(
            version.Version.Value,
            version.Parameter.Value,
            version.ReleaseDate,
            version.Description?.Value
        );
}