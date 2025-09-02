using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;


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
        List<DomainError> errors = [];

        errors
            .CollectErrors<ExampleLink>(link)
            .CollectErrors<StyleName>(styleName)
            .CollectErrors<ModelVersion?>(version);

        if (errors.Count != 0)
            return Result.Fail<MidjourneyStyleExampleLink>(errors);

        var exampleLink = new MidjourneyStyleExampleLink
        (
            link,
            styleName,
            version
        );

        return Result.Ok(exampleLink);
    }
}