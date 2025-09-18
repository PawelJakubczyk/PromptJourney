using Domain.ValueObjects;
using FluentResults;
using Domain.Errors;
using Utilities.Constants;

namespace Domain.Entities;

public class MidjourneyVersion
{
    // Columns
    public ModelVersion Version { get; set; }
    public Param Parameter { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public Description? Description { get; set; }

    // Navigation
    public List<MidjourneyPromptHistory> Histories { get; set; }

    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion1> Versions1 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion2> Versions2 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion3> Versions3 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion4> Versions4 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion5> Versions5 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion51> Versions51 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion52> Versions52 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion6> Versions6 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion61> Versions61 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersion7> Versions7 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji4> VersionsNiji4 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji5> VersionsNiji5 { get; set; }
    public List<MidjourneyAllPropertiesVersions.MidjourneyPropertiesVersionNiji6> VersionsNiji6 { get; set; }

    // Constructors
    private MidjourneyVersion()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyVersion
    (
        ModelVersion version,
        Param parameter,
        DateTime? releaseDate = null,
        Description? description = null
    )
    {
        Version = version;
        Parameter = parameter;
        ReleaseDate = releaseDate;
        Description = description;
    }

    public static Result<MidjourneyVersion> Create
    (
        Result<ModelVersion> versionResult,
        Result<Param> parameterResult,
        DateTime? releaseDate = null,
        Result<Description?>? descriptionResult = null
    )
    {
        List<Error> errors = [];

        errors
            .CollectErrors<DomainLayer, ModelVersion>(versionResult)
            .CollectErrors<DomainLayer, Param>(parameterResult)
            .CollectErrors<DomainLayer, Description>(descriptionResult);

        if (errors.Count != 0)
            return Result.Fail<MidjourneyVersion>(errors);

        var versionMaster = new MidjourneyVersion
        (
            versionResult.Value,
            parameterResult.Value,
            releaseDate,
            descriptionResult?.Value
        );

        return Result.Ok(versionMaster);
    }
}
