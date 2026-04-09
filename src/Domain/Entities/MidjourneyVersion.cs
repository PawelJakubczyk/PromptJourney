using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.Entities;

public sealed class MidjourneyVersion : IEntity
{
    // Columns
    public ModelVersion Version { get; private set; }
    public Param Parameter { get; private set; }
    public ReleaseDate? ReleaseDate { get; private set; }
    public Description? Description { get; private set; }

    // Navigation
    private List<MidjourneyPromptHistory> Histories { get; set; } = [];
    public IReadOnlyCollection<MidjourneyPromptHistory> MidjourneyHistories => Histories.AsReadOnly();

    private List<MidjourneyProperty> Properties { get; set; } = [];
    public IReadOnlyCollection<MidjourneyProperty> MidjourneyProperties => Properties.AsReadOnly();

    // Constructors
    #pragma warning disable CS8618
    private MidjourneyVersion() { } // parameterless constructor for EF Core
    #pragma warning restore CS8618

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
        Result<ReleaseDate> releaseDate,
        Result<Description> descriptionResult
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
                    releaseDate.Value,
                    descriptionResult.Value
                );

                return midjourneyVersion;
            })
            .MapResult<MidjourneyVersion>();

        return result;
    }
}
