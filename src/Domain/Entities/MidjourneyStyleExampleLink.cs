using Domain.Abstractions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Domain.Entities;

public class MidjourneyStyleExampleLink : IEntity
{
    // Primary key
    public Guid Id { get; private set; }

    // Value
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

    private MidjourneyStyleExampleLink
    (
        ExampleLink link,
        StyleName styleName,
        ModelVersion version
    )
    {
        Id = Guid.NewGuid();
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
            .MapResult<MidjourneyStyleExampleLink>();

        return result;
    }

    public static Result<Guid> ParseLinkId(string Id)
    {
        if (!Guid.TryParse(Id, out var linkId))
        {
            return Result.Fail<Guid>("Invalid ID format");
        }

        return Result.Ok(linkId);
    }
}
