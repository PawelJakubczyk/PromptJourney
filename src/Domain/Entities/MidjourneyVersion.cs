using Domain.Abstractions;
using Domain.Extensions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Validation;

namespace Domain.Entities;

public class MidjourneyVersion : IEntitie
{
    // Columns
    public ModelVersion Version { get; set; }
    public Param Parameter { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public Description? Description { get; set; }

    // Navigation
    public List<MidjourneyPromptHistory> Histories { get; set; }

    public List<MidjourneyProperties> Properties { get; set; }


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

    public static Result<MidjourneyVersion> Create(
        Result<ModelVersion> versionResult,
        Result<Param> parameterResult,
        DateTime? releaseDate = null,
        Result<Description?>? descriptionResult = null)
    {
        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .CollectErrors(versionResult)
                .CollectErrors(parameterResult)
                .CollectErrors(descriptionResult))
            .ExecuteIfNoErrors<MidjourneyVersion>(() =>
            {
                var versionMaster = new MidjourneyVersion(
                    versionResult.Value,
                    parameterResult.Value,
                    releaseDate,
                    descriptionResult?.Value
                );

                return versionMaster;
            })
            .MapResult(vm => vm);

        return result;
    }

}
