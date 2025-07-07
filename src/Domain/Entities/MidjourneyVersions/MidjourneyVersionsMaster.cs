using Domain.Entities.MidjourneyPromtHistory;
using System.Linq.Expressions;

namespace Domain.Entities.MidjourneyVersions;

public class MidjourneyVersionsMaster
{
    // Columns
    public required string Version { get; set; }
    public string Parameter { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string? Description { get; set; }

    // Navigation
    public List<MidjourneyPromptHistory> Histories { get; set; }

    public List<MidjourneyAllVersions.MidjourneyVersion1> Versions1 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion2> Versions2 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion3> Versions3 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion4> Versions4 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion5> Versions5 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion51> Versions51 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion52> Versions52 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion6> Versions6 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion61> Versions61 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersion7> Versions7 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersionNiji4> VersionsNiji4 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersionNiji5> VersionsNiji5 { get; set; }
    public List<MidjourneyAllVersions.MidjourneyVersionNiji6> VersionsNiji6 { get; set; }

    // Constructor
    public MidjourneyVersionsMaster()
    {

    }
}
