using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Entities;

public sealed class MidjourneyStyle : IEntity
{
    // Columns
    public StyleName StyleName { get; private set; }
    public StyleType Type { get; private set; }
    public Description? Description { get; private set; }
    public TagsCollection Tags { get; private set; }

    // Navigation properties
    private List<MidjourneyPromptHistory> PromptHistories { get; set; } = [];
    public IReadOnlyCollection<MidjourneyPromptHistory> MidjourneyPromptHistories => PromptHistories.AsReadOnly();

    private List<MidjourneyStyleExampleLink> ExampleLink { get; set; } = [];
    public IReadOnlyCollection<MidjourneyStyleExampleLink> MidjourneyExampleLinks => ExampleLink.AsReadOnly();

    // Constructors
    #pragma warning disable CS8618
    private MidjourneyStyle() { } // parameterless constructor for EF Core
    #pragma warning restore CS8618

    private MidjourneyStyle
    (
        StyleName name,
        StyleType type,
        Description description,
        TagsCollection tags
    )
    {
        StyleName = name;
        Type = type;
        Description = description;
        Tags = tags;
    }

    public static Result<MidjourneyStyle> Create
    (
        Result<StyleName> nameResult,
        Result<StyleType> typeResult,
        Result<Description> descriptionResult,
        Result<TagsCollection> tagsResultsList
    )
    {
        var result = WorkflowPipeline
        .Empty()
        .CongregateErrors(
            pipeline => pipeline.CollectErrors(nameResult),
            pipeline => pipeline.CollectErrors(typeResult),
            pipeline => pipeline.CollectErrors(descriptionResult),
            pipeline => pipeline.CollectErrors(tagsResultsList))
        .ExecuteIfNoErrors<MidjourneyStyle>(() =>
        {
            var style = new MidjourneyStyle
            (
                 nameResult.Value,
                 typeResult.Value,
                 descriptionResult.Value,
                 tagsResultsList.Value
            );
            return style;
        })
        .MapResult<MidjourneyStyle>();

        return result;
    }

    public void AddTag(Result<Tag> tag)
    {
        TagsCollection.AddTag(Tags, tag.Value);
    }

    public void RemoveTag(Result<Tag> tag)
    {
        TagsCollection.RemoveTag(Tags, tag.Value);
    }

    public void UpdateDescription(Result<Description> newDescription)
    {
        Description.Update(newDescription.Value);
    }
}

