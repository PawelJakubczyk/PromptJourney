using Domain.Abstractions;
using Domain.ValueObjects;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Entities;

public class MidjourneyPromptHistory : IEntity
{
    // Columns
    public Guid HistoryId { get; }
    public Prompt Prompt { get; }
    public ModelVersion Version { get; }
    public DateTimeOffset CreatedOn { get; }

    // Navigation
    public MidjourneyVersion MidjourneyVersion { get; set; }
    private List<MidjourneyStyle> Styles { get; set; } = [];
    public IReadOnlyCollection<MidjourneyStyle> MidjourneyStyles => Styles.AsReadOnly();

    // Constructors
    private MidjourneyPromptHistory
    (
        Prompt prompt,
        ModelVersion version
    )
    {
        HistoryId = Guid.NewGuid();
        Prompt = prompt;
        Version = version;
        CreatedOn = DateTimeOffset.UtcNow;
    }

    public static Result<MidjourneyPromptHistory> Create
    (
        Result<Prompt> prompt,
        Result<ModelVersion> version
    )
    {
        var result = WorkflowPipeline
            .Empty()
            .CongregateErrors(
                pipeline => pipeline.CollectErrors(prompt),
                pipeline => pipeline.CollectErrors(version))
            .ExecuteIfNoErrors<MidjourneyPromptHistory>(() => new MidjourneyPromptHistory
            (
                prompt.Value,
                version.Value
            ))
            .MapResult<MidjourneyPromptHistory, MidjourneyPromptHistory>(history => history);

        return result;
    }
}
