namespace Domain.Entities.MidjourneyVersions;
public class MidjourneyVersionsBase
{
    // Columns
    public required string PropertyName { get; set; }
    public required string Version { get; set; }
    public string[]? Modes { get; set; }
    public string? Parameter { get; set; }
    public string[]? ParameterModes { get; set; }
    public string? DefaultValue { get; set; }
    public string? MinValue { get; set; }
    public string? MaxValue { get; set; }
    public string? Description { get; set; }

    // Navigation
    public required MidjourneyVersionsMaster VersionMaster { get; set; }

    // Constructor
    internal MidjourneyVersionsBase()
    {

    }
}
