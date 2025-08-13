using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.Entities.MidjourneyStyles;

public class MidjourneyStyle
{
    // Columns
    public string Name { get; set; }
    public string Type { get; set; }
    public string? Description { get; set; }
    public ICollection<string>? Tags { get; set; }
    
    // Navigation properties
    public List<MidjourneyPromptHistory> MidjourneyPromptHistories { get; set; } = [];
    public List<MidjourneyStyleExampleLink> ExampleLinks { get; set; } = [];

    // Errors
    private static List<DomainError> _errors = [];

    // Constructors
    private MidjourneyStyle()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyStyle
    (
        string name,
        string type, 
        string? description = null, 
        ICollection<string>? tags = null
    )
    {
        Name = name;
        Type = type;
        Description = description;
        Tags = tags!;
    }

    public static Result<MidjourneyStyle> Create
    (
        string name,
        string type,
        string? description = null,
        ICollection<string>? tags = null
    )
    {
        _errors.Clear();

        ValidateName(name);
        ValidateType(type);
        ValidateDescription(description);
        ValidateTags(tags);

        if (_errors.Count > 0) return Result.Fail<MidjourneyStyle>(_errors);
        
        var style = new MidjourneyStyle
        (
            name,
            type,
            description,
            tags
        );

        return Result.Ok(style);
    }

    // Validation methods
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            _errors.Add(NameNullOrEmptyError);
        else if (name.Length > 100)
            _errors.Add(NameToLongError.WithDetail($"name: '{name}' (length: {name.Length})"));
    }

    private static void ValidateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            _errors.Add(TypeNullOrEmptyError);
        else if (type.Length > 50)
            _errors.Add(TypeToLongError.WithDetail($"type: '{type}' (length: {type.Length})"));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length == 0)
            _errors.Add(DescriptionEmptyError);
        else if (description != null && description.Length > 500)
            _errors.Add(DescriptionToLongError.WithDetail($"description length: {description.Length}"));
    }

    private static void ValidateTags(ICollection<string>? tags)
    {
        if (tags?.Count > 10)
            _errors.Add(TagsTooManyError.WithDetail($"tag count: {tags.Count}"));

        foreach (var tag in tags!)
        {
            if (tag.Length > 50)
                _errors.Add(TagTooLongError.WithDetail($"tag: '{tag}' (length: {tag.Length})"));
        }
    }

    private static void ValidateTagExists(ICollection<string> tags, string expectedTag)
    {
        if (!tags.Contains(expectedTag))
        {
            _errors.Add(TagNotFoundError.WithDetail($"tag: '{expectedTag}'"));
        }
    }

    private static void ValidateTagNotExists(ICollection<string> tags, string expectedTag)
    {
        if (tags.Contains(expectedTag))
        {
            _errors.Add(TagDuplicateError.WithDetail($"tag: '{expectedTag}'"));
        }
    }

    // Edit methods
    public Result<MidjourneyStyle> EditName(string newName)
    {
        ValidateName(newName);
        if (_errors.Count > 0) return Result.Fail<MidjourneyStyle>(_errors);

        Name = newName.Trim();
        return Result.Ok(this);
    }    
    
    public Result<MidjourneyStyle>? EditType(string newType)
    {
        ValidateType(newType);
        if (_errors.Count > 0) return Result.Fail<MidjourneyStyle>(_errors);

        Name = newType.Trim();
        return Result.Ok(this);
    }

    public Result<MidjourneyStyle>? EditDescription(string newDescription)
    {
        ValidateDescription(newDescription);
        if (_errors.Count > 0) return Result.Fail<MidjourneyStyle>(_errors);

        Description = newDescription;
        return Result.Ok(this);
    }

    public Result<MidjourneyStyle>? AddTag(string tag)
    {
        ValidateTags([tag]);
        ValidateTagNotExists(Tags!, tag);
        if (_errors.Count > 0) return Result.Fail<MidjourneyStyle>(_errors);

        Tags!.Add(tag);
        return Result.Ok(this);

    }

    public Result<MidjourneyStyle>? RemoveTag(string tag)
    {
        ValidateTags([tag]);
        ValidateTagExists(Tags!, tag);
        if (_errors.Count > 0) return Result.Fail<MidjourneyStyle>(_errors);

        Tags?.Remove(tag);
        return Result.Ok(this);
    }
}