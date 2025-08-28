using Domain.Entities.MidjourneyPromtHistory;
using Domain.Extensions;
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
    public Tag[]? Tags { get; set; }
    
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
        Tag[]? tags = null
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
        Tag[]? tags = null
    )
    {
        List<DomainError> errors = [];

        errors
            .IfEmptyItems<Tag>(tags);

        if (errors.Count != 0)
            return Result.Fail<MidjourneyStyle>(errors);

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