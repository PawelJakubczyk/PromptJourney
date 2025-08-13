using Domain.Entities.MidjourneyPromtHistory;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.Entities.MidjourneyVersions;

public class MidjourneyVersionsMaster
{
    // Columns
    public string Version { get; set; }
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

    // Errors
    private static List<DomainError> _errors = [];

    // Constructor
    private MidjourneyVersionsMaster()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyVersionsMaster
    (
        string version,
        string parameter,
        DateTime? releaseDate = null,
        string? description = null
    )
    {
        Version = version;
        Parameter = parameter;
        ReleaseDate = releaseDate;
        Description = description;
    }

    public static Result<MidjourneyVersionsMaster> Create
    (
        string version,
        string parameter,
        DateTime? releaseDate = null,
        string? description = null
    )
    {
        _errors.Clear();

        ValidateVersion(version);
        ValidateParameter(parameter);
        ValidateReleaseDate(releaseDate);
        ValidateDescription(description);

        if (_errors.Count > 0)
        {
            return Result.Fail<MidjourneyVersionsMaster>(_errors.Select(e => e.Message));
        }

        var versionMaster = new MidjourneyVersionsMaster
        (
            version,
            parameter,
            releaseDate,
            description
        );

        return Result.Ok(versionMaster);
    }

    private static void ValidateVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
            _errors.Add(VersionNullOrEmptyError);
        else if (version.Length > 10)
            _errors.Add(VersionToLongError.WithDetail($"version: '{version}' (length: {version.Length})"));
    }

    private static void ValidateReleaseDate(DateTime? releaseDate)
    {
        if (releaseDate != null && releaseDate > DateTime.Now)
            _errors.Add(ReleaseDateInFutureError.WithDetail($"release date: {releaseDate:yyyy-MM-dd}, current date: {DateTime.Now:yyyy-MM-dd}"));
    }

    private static void ValidateParameter(string? parameter)
    {
        if (string.IsNullOrEmpty(parameter))
            _errors.Add(ParameterNullOrEmptyError);
        if (parameter!.Length > 100)
            _errors.Add(ParameterTooLongError.WithDetail($"parameter: '{parameter}' (length: {parameter.Length})"));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length == 0)
            _errors.Add(DescriptionEmptyError);
        else if (description != null && description.Length > 500)
            _errors.Add(DescriptionToLongError.WithDetail($"description length: {description.Length}"));
    }
}
