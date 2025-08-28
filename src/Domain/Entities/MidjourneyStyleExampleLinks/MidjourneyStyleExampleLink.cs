using Domain.ValueObjects;
using FluentResults;

namespace Domain.Entities.MidjourneyStyles;

public class MidjourneyStyleExampleLink
{
    // Primary key
    public ExampleLink Link { get; private set; }
    
    // Foreign keys
    public StyleName StyleName { get; private set; }
    public ModelVersion Version { get; private set; }
    
    // Navigation
    public MidjourneyStyle Style { get; private set; } = null!;
    public MidjourneyVersions.MidjourneyVersion VersionMaster { get; private set; } = null!;

    // Constructors
    private MidjourneyStyleExampleLink() 
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyStyleExampleLink(ExampleLink link, StyleName styleName, ModelVersion version)
    {
        Link = link;
        StyleName = styleName;
        Version = version;
    }
    
    public static Result<MidjourneyStyleExampleLink> Create
    (
        ExampleLink link,
        StyleName styleName,
        ModelVersion version
    )
    {
        return Result.Ok(new MidjourneyStyleExampleLink(link, styleName, version));
    }
}