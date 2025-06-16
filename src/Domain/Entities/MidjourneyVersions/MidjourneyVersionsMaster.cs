using Domain.Entities.MidjourneyPromtHistory;

namespace Domain.Entities.MidjourneyVersions;

public class MidjourneyVersionsMaster
{
    // Columns
    public required string Version { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? Description { get; set; }

    // Navigation
    public List<MidjourneyPromptHistory> Histories { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion1> Versions1 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion2> Versions2 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion3> Versions3 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion4> Versions4 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion5> Versions5 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion51> Versions51 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion52> Versions52 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion6> Versions6 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion61> Versions61 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersion7> Versions7 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersionNiji4> VersionsNiji4 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersionNiji5> VersionsNiji5 { get; set; }
    public List<MidjourneyAllVersion.MidjourneyVersionNiji6> VersionsNiji6 { get; set; }

    // Constructor
    private MidjourneyVersionsMaster()
    {

    }
}
