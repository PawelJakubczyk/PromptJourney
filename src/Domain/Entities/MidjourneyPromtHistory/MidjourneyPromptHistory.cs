using Domain.Entities.MidjourneyStyles;
using Domain.Errors;
using Domain.ValueObjects;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;


namespace Domain.Entities.MidjourneyPromtHistory;

public class MidjourneyPromptHistory
{
    // Columns
    public Guid HistoryId { get; }
    public Prompt Prompt { get; }
    public ModelVersion Version { get; }
    public DateTime CreatedOn { get; }

    // Navigation
    public MidjourneyVersions.MidjourneyVersion VersionMaster { get; set; }
    public List<MidjourneyStyle> MidjourneyStyles { get; set; } = [];

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
        var historyId = Guid.NewGuid();
        var createdOn = DateTime.UtcNow;

        HistoryId = historyId;
        Prompt = prompt;
        Version = version;
        CreatedOn = createdOn;
    }

    public static Result<MidjourneyPromptHistory> Create
    (
        Prompt prompt,
        ModelVersion version
    )
    {
        List<DomainError> errors = [];

        errors
            .CollectErrors<Prompt>(prompt)
            .CollectErrors<ModelVersion>(version);

        if (errors.Count != 0)
            return Result.Fail<MidjourneyPromptHistory>(errors);

        var history = new MidjourneyPromptHistory
        (
            prompt,
            version
        );

        return Result.Ok(history);
    }
}





