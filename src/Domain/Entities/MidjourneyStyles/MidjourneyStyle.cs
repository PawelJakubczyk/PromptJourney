using Domain.Entities.MidjourneyPromtHistory;

namespace Domain.Entities.MidjourneyStyles;

public class MidjourneyStyle
{
    // Columns
    public required string Name { get; set; }
    public string Type { get; set; }
    public string? Description { get; set; }
    public ICollection<string>? Tags { get; set; }
    public ICollection<string> ExampleLinks { get; set; }

    // Navigation
    public required List<MidjourneyPromptHistory> MidjourneyPromptHistories { get; set; } = [];

    // Constructor
    public MidjourneyStyle(string name, string type, string? description = null, ICollection<string>? tags = null, ICollection<string>? exampleLinks = null)
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
        if (!Tags.Contains(tag))
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
