using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Constants;

namespace Domain.Entities;

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
        Result<StyleName> nameResult,
        Result<StyleType> typeResult,
        Result<Description?>? descriptionResult = null,
        List<Result<Tag>>? tagResultsList = null
    )
    {
        List<Error> errors = [];

        errors
            .CollectErrors<DomainLayer, StyleName>(nameResult)
            .CollectErrors<DomainLayer, StyleType>(typeResult)
            .CollectErrors<DomainLayer, Description>(descriptionResult);
            
        List<Tag>? tagsList = null;

        if (tagResultsList != null && tagResultsList.Count > 0)
        {
            tagsList = [];
            foreach (var tagResult in tagResultsList)
            {
                errors.CollectErrors<DomainLayer, Tag>(tagResult);
                if (tagResult.IsSuccess)
                {
                    errors.IfTagAllredyExist<DomainLayer>(tagsList, tagResult.Value);
                    tagsList.Add(tagResult.Value);
                }
            }
        }

        if (errors.Count != 0)
            return Result.Fail<MidjourneyStyle>(errors);

        var style = new MidjourneyStyle
        (
            nameResult.Value,
            typeResult.Value,
            descriptionResult?.Value,
            tagsList
        );

        return Result.Ok(style);
    }

    public Result AddTag(Result<Tag> tag)
    {
        List<Error> errors = [];

        errors
            .CollectErrors<DomainLayer, ModelVersion>(tag)
            .IfTagAllredyExist<DomainLayer, ModelVersion>(Tags, tag.Value);

        if (errors.Count != 0)
            return Result.Fail(errors);

        Tags ??= [];
        Tags.Add(tag.Value);

        return Result.Ok();
    }

    public Result RemoveTag(Result<Tag> tag)
    {
        List<Error> errors = [];

        errors
            .CollectErrors<DomainLayer, Tag>(tag)
            .IfListIsEmpty<DomainLayer, Tag>(Tags)
            .IfNull<DomainLayer, Tag>(Tags)
            .IfListNotContain<DomainLayer, Tag>(Tags, tag.Value);

        if (errors.Count != 0)
            return Result.Fail(errors);

        Tags!.RemoveAll(t => t.Equals(tag.Value));

        return Result.Ok();
    }

    public Result EditDescription(Result<Description?> description)
    {
        List<Error> errors = [];

        errors.CollectErrors<DomainLayer, ModelVersion>(description);

        if (errors.Count != 0)
            return Result.Fail(errors);

        Description = description.Value;

        return Result.Ok();
    }

    public override int GetHashCode()
    {
        return StyleName.GetHashCode();
    }
}