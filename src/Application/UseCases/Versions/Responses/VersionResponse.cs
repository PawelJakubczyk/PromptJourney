using Domain.Entities;

namespace Application.UseCases.Versions.Responses;

public sealed record VersionResponse
(
    string Version,
    string Parameter,
    string? ReleaseDate,
    string? Description
)
{
    public static VersionResponse FromDomain(MidjourneyVersion version) =>
        new(
            version.Version.Value,
            version.Parameter.Value,
            version.ReleaseDate.Value,
            version.Description?.Value
        );
}