using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Entities;

public sealed class MidjourneyPromptHistory : IEntity
{
    // Columns
    public HistoryID HistoryId { get; private set; }
    public Prompt Prompt { get; private set; }
    public ModelVersion Version { get; private set; }
    public CreatedOn CreatedOn { get; private set; }

    // Navigation
    public MidjourneyVersion MidjourneyVersion { get; private set; } = null!;
    private List<MidjourneyStyle> Styles { get; set; } = [];
    public IReadOnlyCollection<MidjourneyStyle> MidjourneyStyles => Styles.AsReadOnly();

    // Constructors
    #pragma warning disable CS8618
    private MidjourneyPromptHistory() { } // parameterless constructor for EF Core
    #pragma warning restore CS8618

    private MidjourneyPromptHistory
    (
        HistoryID historyId,
        Prompt prompt,
        ModelVersion version,
        CreatedOn createdOn
    )
    {
        HistoryId = historyId;
        Prompt = prompt;
        Version = version;
        CreatedOn = createdOn;
    }

    public static Result<MidjourneyPromptHistory> Create
    (
        Result<HistoryID> historyId,
        Result<Prompt> prompt,
        Result<ModelVersion> version,
        Result<CreatedOn> createdOn
    )
    {
        var result = WorkflowPipeline
            .Empty()
            .CongregateErrors(
                pipeline => pipeline.CollectErrors(historyId),
                pipeline => pipeline.CollectErrors(prompt),
                pipeline => pipeline.CollectErrors(version))
            .ExecuteIfNoErrors<MidjourneyPromptHistory>(() => new MidjourneyPromptHistory
            (
                historyId.Value,
                prompt.Value,
                version.Value,
                createdOn.Value
            ))
            .MapResult<MidjourneyPromptHistory, MidjourneyPromptHistory>(history => history);

        return result;
    }
}
