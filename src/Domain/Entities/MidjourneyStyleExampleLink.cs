using Domain.Abstractions;
using Domain.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Constants;
using Utilities.Validation;

namespace Domain.Entities;

public class MidjourneyStyleExampleLink : IEntitie
{
    // Primary key
    public ExampleLink Link { get; private set; }

    // Foreign keys
    public StyleName StyleName { get; private set; }
    public ModelVersion Version { get; private set; }

    // Navigation
    public MidjourneyStyle Style { get; private set; } = null!;
    public MidjourneyVersion VersionMaster { get; private set; } = null!;

    // Constructors
    private MidjourneyStyleExampleLink()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyStyleExampleLink(
        ExampleLink link,
        StyleName styleName,
        ModelVersion version)
    {
        Link = link;
        StyleName = styleName;
        Version = version;
    }

    public static Result<MidjourneyStyleExampleLink> Create(
        Result<ExampleLink>? linkResult,
        Result<StyleName>? styleNameResult,
        Result<ModelVersion>? versionResult)
    {
        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .CollectErrors(linkResult)
                .CollectErrors(styleNameResult)
                .CollectErrors(versionResult))
            .ExecuteIfNoErrors<MidjourneyStyleExampleLink>(() =>
            {
                var exampleLink = new MidjourneyStyleExampleLink(
                    linkResult?.Value!,
                    styleNameResult?.Value!,
                    versionResult?.Value!
                );

                return exampleLink;
            })
            .MapResult(link => link);

        return result;
    }
}
