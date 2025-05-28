using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.MidjourneyVersions;
public class MidjourneyVersionsBase
{
    // Columns
    [Key]
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

public class MidjourneyVersion1 : MidjourneyVersionsBase { }
public class MidjourneyVersion2 : MidjourneyVersionsBase { }
public class MidjourneyVersion3 : MidjourneyVersionsBase { }
public class MidjourneyVersion4 : MidjourneyVersionsBase { }
public class MidjourneyVersion5 : MidjourneyVersionsBase { }
public class MidjourneyVersion51 : MidjourneyVersionsBase { }
public class MidjourneyVersion52 : MidjourneyVersionsBase { }
public class MidjourneyVersion6 : MidjourneyVersionsBase { }
public class MidjourneyVersion61 : MidjourneyVersionsBase { }
public class MidjourneyVersion7 : MidjourneyVersionsBase { }
public class MidjourneyVersionNiji4 : MidjourneyVersionsBase { }
public class MidjourneyVersionNiji5 : MidjourneyVersionsBase { }
public class MidjourneyVersionNiji6 : MidjourneyVersionsBase { }
