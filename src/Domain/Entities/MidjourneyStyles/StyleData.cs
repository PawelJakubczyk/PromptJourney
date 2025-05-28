using Domain.Entities.MidjourneyPromtHistory;

namespace Domain.Entities.MidjourneyStyles;

public class StyleData
{
    // Columns
    public Guid HistoryID { get; set; }
    public string? MidjourneyStyleName { get; set; }

    // Navigation
    public required MidjourneyPromptHistory MidjourneyPromptHistory { get; set; }
    public MidjourneyStyle? MidjourneyStyle { get; set; }

    // Constructor
    public StyleData()
    {

    }
}
