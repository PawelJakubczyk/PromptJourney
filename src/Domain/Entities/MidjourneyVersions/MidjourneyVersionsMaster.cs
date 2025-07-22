using Domain.Entities.MidjourneyPromtHistory;
using Domain.Entities.MidjourneyVersions.Exceptions;
using Domain.ErrorMassages;
using Domain.Exceptions;
using FluentResults;

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
    private static List<MidjourneyEntitiesException> _errors = [];

    // Constructor
    private MidjourneyVersionsMaster()
    {

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
            _errors.Add(new VersionValidationException("Version", ErrorMessages.VersionNullOrEmpty));
        else if (version.Length > 10)
            _errors.Add(new VersionValidationException("Version", ErrorMessages.VersionTooLong));
    }

    private static void ValidateReleaseDate(DateTime? releaseDate)
    {
        if (releaseDate != null && releaseDate > DateTime.Now)
            _errors.Add(new VersionValidationException("ReleaseDate", ErrorMessages.ReleaseDateInFuture));
    }

    private static void ValidateParameter(string? parameter)
    {
        if (string.IsNullOrEmpty(parameter))
            _errors.Add(new VersionValidationException("Parameter", ErrorMessages.ParameterNullOrEmpty));
        if (parameter!.Length > 100)
            _errors.Add(new VersionValidationException("Parameter", ErrorMessages.ParameterTooLong));
    }

    private static void ValidateDescription(string? description)
    {
        if (description != null && description.Length == 0)
            _errors.Add(new VersionValidationException("Description", ErrorMessages.DescriptionEmpty));
        else if (description != null && description.Length > 500)
            _errors.Add(new VersionValidationException("Description", ErrorMessages.DescriptionTooLong));
    }
}
