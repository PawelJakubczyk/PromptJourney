using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Entities;

public class MidjourneyStyle : IEntity
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
    private MidjourneyStyle
    (
        StyleName name,
        StyleType type,
        Description? description,
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
        Result<Description>? descriptionResult = null,
        Result<TagsCollection>? tagsResultsList = null
    )
    {
        var descriptionResultNonNull = descriptionResult ?? Result<Description>.Ok(Description.None);
        var tagsResultNonNull = tagsResultsList ?? Result<TagsCollection>.Ok(TagsCollection.None);

        var result = WorkflowPipeline
        .Empty()
        .CongregateErrors(
            pipeline => pipeline.CollectErrors(nameResult),
            pipeline => pipeline.CollectErrors(typeResult),
            pipeline => pipeline.CollectErrors(descriptionResultNonNull),
            pipeline => pipeline.CollectErrors(tagsResultNonNull))
        .ExecuteIfNoErrors<MidjourneyStyle>(() =>
        {
            var style = new MidjourneyStyle
            (
                 nameResult.Value,
                 typeResult.Value,
                 descriptionResultNonNull.Value,
                 tagsResultNonNull.Value
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

