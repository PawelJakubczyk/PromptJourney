using Domain.Abstractions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Domain.Entities;

public class MidjourneyVersion : IEntity
{
    // Columns
    public ModelVersion Version { get; set; }
    public Param Parameter { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public Description? Description { get; set; }

    // Navigation
    public List<MidjourneyPromptHistory> MidjourneyHistories { get; set; }

    public List<MidjourneyProperties> MidjourneyProperties { get; set; }

    // Constructors
    private MidjourneyVersion()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyVersion(
        ModelVersion version,
        Param parameter,
        DateTime? releaseDate = null,
        Description? description = null)
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
        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .CollectErrors(versionResult)
                .CollectErrors(parameterResult)
                .CollectErrors(descriptionResult))
            .ExecuteIfNoErrors<MidjourneyVersion>(() =>
            {
                var midjourneyVersion = new MidjourneyVersion(
                    versionResult.Value,
                    parameterResult.Value,
                    releaseDate,
                    descriptionResult?.Value
                );

                return midjourneyVersion;
            })
            .MapResult<MidjourneyVersion>();

        return result;
    }
}
