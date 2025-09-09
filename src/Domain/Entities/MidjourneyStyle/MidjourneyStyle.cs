using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyStyleExampleLinks;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;

namespace Domain.Entities.MidjourneyStyle;

public class MidjourneyStyle
{
    // Columns
    public StyleName StyleName { get; set; }
    public StyleType Type { get; set; }
    public Description? Description { get; set; }
    public List<Tag>? Tags { get; set; }
    
    // Navigation properties
    public List<MidjourneyPromptHistory> MidjourneyPromptHistories { get; set; } = [];
    public List<MidjourneyStyleExampleLink> ExampleLinks { get; set; } = [];

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
        StyleName name,
        StyleType type,
        Description? description = null,
        List<Tag>? tags = null
    )
    {
        List<DomainError> errors = [];

        errors
            .CollectErrors<StyleName>(name)
            .CollectErrors<StyleType>(type)
            .CollectErrors<Description?>(description);
            
        foreach (var tag in tags ?? [])
        {
            errors.CollectErrors<Tag>(tag);
        }

        if (errors.Count != 0)
            return Result.Fail<MidjourneyStyle>(errors);

        if (tags?.Count == 0)
        {
            tags = null;
        }

        var style = new MidjourneyStyle
        (
            name,
            type,
            description,
            tags
        );

        return Result.Ok(style);
    }

    public Result AddTag(Tag tag)
    {
        List<DomainError> errors = [];

        errors
            .CollectErrors<Tag>(tag)
            .IfCountain<Tag>(Tags, tag);

        if (errors.Count != 0)
            return Result.Fail(errors);

        Tags ??= [];
        Tags.Add(tag);

        return Result.Ok();
    }

    public Result RemoveTag(Tag tag)
    {
        List<DomainError> errors = [];

        errors
            .CollectErrors<Tag>(tag)
            .IfListIsEmpty<Tag>(Tags)
            .IfNull<Tag>(Tags)
            .IfDoesNotContain(Tags, tag);

        if (errors.Count != 0)
            return Result.Fail(errors);

        Tags!.RemoveAll(t => t.Equals(tag));

        return Result.Ok();
    }

    public Result EditDescription(Description? description)
    {
        List<DomainError> errors = [];

        errors.CollectErrors<Description?>(description);

        if (errors.Count != 0)
            return Result.Fail(errors);

        Description = description;

        return Result.Ok();
    }
}