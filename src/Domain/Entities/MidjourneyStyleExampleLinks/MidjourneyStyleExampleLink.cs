using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;


namespace Domain.Entities.MidjourneyStyleExampleLinks;

public class MidjourneyStyleExampleLink
{
    // Primary key
    public ExampleLink Link { get; private set; }
    
    // Foreign keys
    public StyleName StyleName { get; private set; }
    public ModelVersion Version { get; private set; }
    
    // Navigation
    public MidjourneyStyle.MidjourneyStyle Style { get; private set; } = null!;
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
        Result<ExampleLink> link,
        Result<StyleName> styleName,
        Result<ModelVersion> version
    )
    {
        List<DomainError> errors = [];

        errors
            .CollectErrors<ExampleLink>(link)
            .CollectErrors<StyleName>(styleName)
            .CollectErrors<ModelVersion>(version);

        if (errors.Count != 0)
            return Result.Fail<MidjourneyStyleExampleLink>(errors);

        var exampleLink = new MidjourneyStyleExampleLink
        (
            link.Value,
            styleName.Value,
            version.Value
        );

        return Result.Ok(exampleLink);
    }
}