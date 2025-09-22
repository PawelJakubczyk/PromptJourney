using Domain.Abstractions;
using Domain.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;

namespace Domain.Entities;

public class MidjourneyStyle : IEntitie
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
        List<Result<Tag>>? tagsResultsList = null
    )
    {
        List<Error> errors = [];

        errors
            .CollectErrors<StyleName>(nameResult)
            .CollectErrors<StyleType>(typeResult)
            .CollectErrors<Description>(descriptionResult)
            .CollectErrors<Tag>(tagsResultsList);

        var tagsList = tagsResultsList.ToValueList();

        errors
            .IfListHasDuplicates<DomainLayer, Tag>(tagsList);

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
            .CollectErrors<Tag>(tag)
            .IfListContain<DomainLayer, Tag>(Tags, tag.Value);

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
            .CollectErrors<Tag>(tag)
            .IfListIsEmpty<DomainLayer, Tag>(Tags)
            .IfListIsNull<DomainLayer, Tag>(Tags)
            .IfListNotContain<DomainLayer, Tag>(Tags, tag.Value);

        if (errors.Count != 0)
            return Result.Fail(errors);

        Tags!.RemoveAll(t => t.Equals(tag.Value));

        return Result.Ok();
    }

    public Result EditDescription(Result<Description?> description)
    {
        List<Error> errors = [];

        errors.CollectErrors<Description>(description);

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

internal static class MidjourneyStyleErrorsExtensions
{
    public static List<Error> IfListIsNull<TLayer, TValue>(this List<Error> Errors, List<TValue?> value)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (value is null)
        {
            Errors.Add(new Error<TLayer>($"List of {typeof(TValue).Name}: cannot be null."));
        }
        return Errors;
    }

    public static List<Error> IfListIsEmpty<TLayer, TValue>(this List<Error> Errors, List<TValue>? items)
    where TLayer : ILayer
    where TValue : ValueObject<string>
    {
        if (items != null && items.Count == 0)
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: Cannot be an empty collection."));
        }
        return Errors;
    }

    public static List<Error> IfListNotContain<TLayer, TValue>(this List<Error> Errors, List<TValue>? items, TValue element)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (items != null && items.Contains(element) == false)
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: Collection does not contain the required element."));
        }
        return Errors;
    }

    public static List<Error> IfListContain<TLayer, TValue>(this List<Error> Errors, List<TValue>? items, TValue element)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (items != null && items.Contains(element))
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: Collection already contains the element."));
        }
        return Errors;
    }
}