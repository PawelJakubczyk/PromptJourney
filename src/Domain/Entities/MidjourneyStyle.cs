using Domain.Abstractions;
using Domain.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.Entities;

public class MidjourneyStyle : IEntity
{
    // Columns
    public StyleName StyleName { get; set; }
    public StyleType Type { get; set; }
    public Description? Description { get; set; }
    public List<Tag>? Tags { get; set; }

    // Navigation properties
    private List<MidjourneyPromptHistory> PromptHistories { get; set; } = [];
    public IReadOnlyCollection<MidjourneyPromptHistory> MidjourneyPromptHistories => PromptHistories.AsReadOnly();

    private List<MidjourneyStyleExampleLink> ExampleLink { get; set; } = [];
    public IReadOnlyCollection<MidjourneyStyleExampleLink> MidjourneyExampleLinks => ExampleLink.AsReadOnly();

    // Constructors
    private MidjourneyStyle()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyStyle
    (
        StyleName name,
        StyleType type,
        Description? description = null,
        List<Tag>? tags = null
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
        Result<Description?>? descriptionResult = null,
        List<Result<Tag>>? tagsResultsList = null
    )
    {
        var result = WorkflowPipeline
        .Empty()
        .Congregate(pipeline => pipeline
            .CollectErrors(nameResult)
            .CollectErrors(typeResult)
            .CollectErrors(descriptionResult)
            .CollectErrors(tagsResultsList)
            .IfListIsEmpty<DomainLayer, Tag>(tagsResultsList?.ToValueList())
            .IfListHasDuplicates<DomainLayer, Tag>(tagsResultsList?.ToValueList()))
        .ExecuteIfNoErrors<MidjourneyStyle>(() =>
        {
            var style = new MidjourneyStyle
            (
                 nameResult.Value,
                 typeResult.Value,
                 descriptionResult?.Value,
                 tagsResultsList?.ToValueList() ?? null
            );
            return style;
        })
        .MapResult<MidjourneyStyle>();

        return result;
    }

    public Result<Tag> AddTag(Result<Tag> tag)
    {
        var result = WorkflowPipeline
            .Empty()
            .Congregate(pipeline => pipeline
                .CollectErrors(tag)
                .IfListContain<DomainLayer, Tag>(Tags, tag.Value))
            .ExecuteIfNoErrors<Tag>(() =>
            {
                Tags ??= [];
                Tags.Add(tag.Value);
                return tag;
            })
            .MapResult<Tag>();

        return result;
    }

    public Result<Tag> RemoveTag(Result<Tag> tag)
    {
        var result = WorkflowPipeline
            .Empty()
            .Congregate(pipeline => pipeline
                .CollectErrors(tag)
                .IfListIsNull<DomainLayer, Tag>(Tags)
                .IfListIsEmpty<DomainLayer, Tag>(Tags)
                .IfListNotContain<DomainLayer, Tag>(Tags, tag.Value))
            .ExecuteIfNoErrors(() =>
            {
                Tags!.RemoveAll(t => t.Equals(tag.Value));
                return tag;
            })
            .MapResult<Tag>();

        return result;
    }

    public Result<Description> EditDescription(Result<Description?>? description)
    {
        var result = WorkflowPipeline
            .Empty()
            .CollectErrors(description)
            .ExecuteIfNoErrors<Description>(() =>
            {
                Description = description?.Value;
                return description?.Value;
            })
            .MapResult<Description>();

        return result;
    }
}

