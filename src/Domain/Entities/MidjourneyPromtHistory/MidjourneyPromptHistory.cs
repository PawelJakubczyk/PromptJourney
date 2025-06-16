using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
//using Domain.Entities.MidjourneyVersions;

namespace Domain.Entities.MidjourneyPromtHistory;

public class MidjourneyPromptHistory
{
    // Columns
    public Guid HistoryId { get; set; }
    public required string Prompt { get; set; }
    public required string Version { get; set; }
    public required DateTime CreatedOn { get; set; }

    // Navigation
    public required MidjourneyVersionsMaster VersionMaster { get; set; }
    public List<MidjourneyStyle> MidjourneyStyles { get; set; } = [];

    // Constructor
    public MidjourneyPromptHistory(Guid historyId, string prompt, string version, DateTime createdOn)
    {
        HistoryId = historyId;
        Prompt = prompt;
        Version = version;
        CreatedOn = createdOn;
    }

    private MidjourneyPromptHistory()
    {

    }
}
