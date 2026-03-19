using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.Entities;

public sealed class MidjourneyStyleExampleLink : IEntity
{
    // Primary key
    public LinkID Id { get; private set; }

    // Value
    public ExampleLink Link { get; private set; }

    // Foreign keys
    public StyleName StyleName { get; private set; }
    public ModelVersion Version { get; private set; }

    // Navigation
    public MidjourneyStyle MidjuorneyStyle { get; private set; } = null!;
    public MidjourneyVersion MidjourneyMaster { get; private set; } = null!;

    // Constructors
    private MidjourneyStyleExampleLink
    (
        LinkID id,
        ExampleLink link,
        StyleName styleName,
        ModelVersion version
    )
    {
        Id = id;
        Link = link;
        StyleName = styleName;
        Version = version;
    }

    public static Result<MidjourneyStyleExampleLink> Create
    (
        Result<LinkID> idResult,
        Result<ExampleLink> linkResult,
        Result<StyleName> styleNameResult,
        Result<ModelVersion> versionResult
    )
    {
        var result = WorkflowPipeline
            .Empty()
            .CongregateErrors(
                pipeline => pipeline.CollectErrors(idResult),
                pipeline => pipeline.CollectErrors(linkResult),
                pipeline => pipeline.CollectErrors(styleNameResult),
                pipeline => pipeline.CollectErrors(versionResult))
            .ExecuteIfNoErrors<MidjourneyStyleExampleLink>(() =>
            {
                var exampleLink = new MidjourneyStyleExampleLink(
                    idResult.Value,
                    linkResult.Value,
                    styleNameResult.Value,
                    versionResult.Value
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
