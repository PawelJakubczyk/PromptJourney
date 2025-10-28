using Domain.Abstractions;
using Domain.ValueObjects;
using FluentResults;
using Utilities.Workflows;

namespace Domain.Entities;

public class MidjourneyPromptHistory : IEntity
{
    // Columns
    public Guid HistoryId { get; }
    public Prompt Prompt { get; }
    public ModelVersion Version { get; }
    public DateTime CreatedOn { get; }

    // Navigation
    public MidjourneyVersion MidjourneyVersion { get; set; }
    private List<MidjourneyStyle> Styles { get; set; } = [];
    public IReadOnlyCollection<MidjourneyStyle> MidjourneyStyles => Styles.AsReadOnly();

    // Constructors
    private MidjourneyPromptHistory()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyPromptHistory
    (
        Prompt prompt,
        ModelVersion version
    )
    {
        HistoryId = Guid.NewGuid();
        Prompt = prompt;
        Version = version;
        CreatedOn = DateTime.UtcNow;
    }

    public static Result<MidjourneyPromptHistory> Create
    (
        Result<Prompt> prompt,
        Result<ModelVersion> version,
        DateTime? createdOn = null
    )
    {
        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .CollectErrors(prompt)
                .CollectErrors(version))
            .ExecuteIfNoErrors<MidjourneyPromptHistory>(() => new MidjourneyPromptHistory
            (
                prompt.Value,
                version.Value
            ))
            .MapResult<MidjourneyPromptHistory, MidjourneyPromptHistory>(history => history);

        return result;
    }

    public void AddStyle(MidjourneyStyle style)
    {
        if (!Styles.Contains(style))
        {
            Styles.Add(style);
        }
    }
}
