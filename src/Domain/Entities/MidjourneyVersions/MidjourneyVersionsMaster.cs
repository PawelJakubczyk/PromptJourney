using Domain.Entities.MidjourneyPromtHistory;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.MidjourneyVersions;

public class MidjourneyVersionsMaster
{
    // Columns
    [Key]
    public required string Version { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? Description { get; set; }

    // Navigation
    public required ICollection<MidjourneyVersionsBase> AllVersions { get; set; }
    public required ICollection<MidjourneyPromptHistory> PromptHistories { get; set; }

    // Constructor
    private MidjourneyVersionsMaster()
    {

    }
}
