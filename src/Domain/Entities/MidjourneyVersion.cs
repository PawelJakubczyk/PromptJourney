using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.Entities;

public class MidjourneyVersion : IEntity
{
    // Columns
    public ModelVersion Version { get; set; }
    public Param Parameter { get; set; }
    public ReleaseDate? ReleaseDate { get; set; }
    public Description? Description { get; set; }

    // Navigation
    private List<MidjourneyPromptHistory> Histories { get; set; } = [];
    public IReadOnlyCollection<MidjourneyPromptHistory> MidjourneyHistories => Histories.AsReadOnly();

    private List<MidjourneyProperties> Properties { get; set; } = [];
    public IReadOnlyCollection<MidjourneyProperties> MidjourneyProperties => Properties.AsReadOnly();

    // Constructors
    private MidjourneyVersion
    (
        ModelVersion version,
        Param parameter,
        ReleaseDate? releaseDate,
        Description? description
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
        Result<ReleaseDate?> releaseDate = null,
        Result<Description?> descriptionResult = null
    )
    {
        var result = WorkflowPipeline
            .Empty()
            .CongregateErrors(
                pipeline => pipeline.CollectErrors(versionResult),
                pipeline => pipeline.CollectErrors(parameterResult),
                pipeline => pipeline.CollectErrors(releaseDate),
                pipeline => pipeline.CollectErrors(descriptionResult))
            .ExecuteIfNoErrors<MidjourneyVersion>(() =>
            {
                var midjourneyVersion = new MidjourneyVersion(
                    versionResult.Value,
                    parameterResult.Value,
                    releaseDate.ValueOr(null),
                    descriptionResult?.ValueOr(null)
                );

                return midjourneyVersion;
            })
            .MapResult<MidjourneyVersion>();

        return result;
    }
}
