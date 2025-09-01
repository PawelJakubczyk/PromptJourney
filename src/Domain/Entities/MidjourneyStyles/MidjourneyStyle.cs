using Domain.Entities.MidjourneyPromtHistory;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.Entities.MidjourneyStyles;

public class MidjourneyStyle
{
    // Columns
    public StyleName Name { get; set; }
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
        Name = name;
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
            .CollectErrors<Description?>(description)
            .CollectErrors<List<Tag>?>(tags);

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
}