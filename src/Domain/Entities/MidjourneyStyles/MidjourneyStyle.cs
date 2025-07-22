using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyVersions;
using Domain.Entities.MidjourneyVersions.Exceptions;
using Domain.ErrorMassages;
using Domain.Exceptions;
using FluentResults;
using System.Xml.Linq;

namespace Domain.Entities.MidjourneyStyles;

public class MidjourneyStyle
{
    // Columns
    public string Name { get; set; }
    public string Type { get; set; }
    public string? Description { get; set; }
    public ICollection<string>? Tags { get; set; }
    public ICollection<string> ExampleLinks { get; set; }

    // Navigation
    public List<MidjourneyPromptHistory> MidjourneyPromptHistories { get; set; } = [];

    // Errors
    private static List<MidjourneyEntitiesException> _errors = [];

    // Constructor
    private MidjourneyStyle
    (
        string name,
        string type, 
        string? description = null, 
        ICollection<string>? tags = null,
        ICollection<string>? exampleLinks = null
    )
    {
        Name = name;
        Type = type;
        Description = description;
        Tags = tags!;
        ExampleLinks = exampleLinks!;
    }

    private MidjourneyStyle()
    {

    }


    public static Result<MidjourneyStyle> Create
    (
        string name,
        string type,
        string? description = null,
        ICollection<string>? tags = null,
        ICollection<string>? exampleLinks = null
    )
    {
        ValidateName(name);
        ValidateType(type);
        ValidateDescription(description);
        ValidateTags(tags);
        ValidateExampleLinks(exampleLinks);

        if (_errors.Count > 0)
        {
            return Result.Fail<MidjourneyStyle>(_errors.Select(e => e.Message));
        }

        var style = new MidjourneyStyle
        (
        name,
        type,
        description = null,
        tags = null,
        exampleLinks = null
        );

        return Result.Ok(style);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            _errors.Add(new VersionValidationException("Name", ErrorMessages.NameEmpty));
        else if (name.Length > 100)
            _errors.Add(new VersionValidationException("Name", ErrorMessages.NameTooLong));
    }

    private static void ValidateType(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
            _errors.Add(new VersionValidationException("Type", ErrorMessages.TypeEmpty));
        else if (type.Length > 50)
            _errors.Add(new VersionValidationException("Type", ErrorMessages.TypeTooLong));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length == 0)
            _errors.Add(new VersionValidationException("Description", ErrorMessages.DescriptionEmpty));
        else if (description != null && description.Length > 500)
            _errors.Add(new VersionValidationException("Description", ErrorMessages.DescriptionTooLong));
    }

    private static void ValidateTags(ICollection<string>? tags)
    {
        if (tags == null || tags.Count == 0)
            _errors.Add(new VersionValidationException("Tags", ErrorMessages.TagsEmpty));
        else if (tags.Count > 10)
            _errors.Add(new VersionValidationException("Tags", ErrorMessages.TagsTooMany));

        foreach (var tag in tags!)
        {
            if (string.IsNullOrWhiteSpace(tag))
                _errors.Add(new VersionValidationException("Tag", ErrorMessages.TagEmpty));
            else if (tag.Length > 50)
                _errors.Add(new VersionValidationException("Tag", ErrorMessages.TagTooLong));
        }
    }

    private static void ValidateExampleLinks(ICollection<string>? exampleLinks)
    {
        if (exampleLinks == null || exampleLinks.Count == 0)
            _errors.Add(new VersionValidationException("ExampleLinks", ErrorMessages.ExampleLinksEmpty));
        else if (exampleLinks.Count > 10)
            _errors.Add(new VersionValidationException("ExampleLinks", ErrorMessages.ExampleLinksTooMany));

        foreach (var link in exampleLinks!)
        {
            if (string.IsNullOrWhiteSpace(link))
                _errors.Add(new VersionValidationException("ExampleLink", ErrorMessages.ExampleLinkEmpty));
            else if (link.Length > 200)
                _errors.Add(new VersionValidationException("ExampleLink", ErrorMessages.ExampleLinkTooLong));
        }
    }






    public void EditName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty.", nameof(newName));

        Name = newName.Trim();
    }    
    
    public void EditType(string newType)
    {
        if (string.IsNullOrWhiteSpace(newType))
            throw new ArgumentException("Type cannot be empty.", nameof(newType));

        Name = newType.Trim();
    }

    public void EditDescription(string newDescription)
    {
        Description = newDescription;
    }

    public void AddTag(string tag)
    {
        if (!Tags!.Contains(tag))
            Tags.Add(tag);
    }

    public void RemoveTag(string tag)
    {
        Tags?.Remove(tag);
    }

    public void AddLink(string link)
    {
        if (!ExampleLinks.Contains(link))
            ExampleLinks.Add(link);
    }

    public void RemoveLink(string link)
    {
        ExampleLinks.Remove(link);
    }
}
