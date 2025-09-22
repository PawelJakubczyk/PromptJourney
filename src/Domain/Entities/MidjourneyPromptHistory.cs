using Domain.Abstractions;
using Domain.Extensions;
using Domain.ValueObjects;
using FluentResults;

namespace Domain.Entities;

public class MidjourneyPromptHistory: IEntitie
{
    // Columns
    public Guid HistoryId { get; }
    public Prompt Prompt { get; }
    public ModelVersion Version { get; }
    public DateTime CreatedOn { get; }

    // Navigation
    public MidjourneyVersion VersionMaster { get; set; }
    public List<MidjourneyStyle> MidjourneyStyles { get; set; } = [];

    // Constructors
    private MidjourneyPromptHistory()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyPromptHistory
    (
        Prompt prompt,
        ModelVersion version,
        DateTime? createdOn = null
    )
    {
        var historyId = Guid.NewGuid();
        var creationTime = createdOn ?? DateTime.UtcNow;

        HistoryId = historyId;
        Prompt = prompt;
        Version = version;
        CreatedOn = creationTime;
    }

    public static Result<MidjourneyPromptHistory> Create
    (
        Result<Prompt> prompt,
        Result<ModelVersion> version,
        DateTime? createdOn = null
    )
    {
        List<Error> errors = [];

        errors
            .CollectErrors<Prompt>(prompt)
            .CollectErrors<ModelVersion>(version);

        if (errors.Count != 0)
            return Result.Fail<MidjourneyPromptHistory>(errors);

        var history = new MidjourneyPromptHistory
        (
            prompt.Value,
            version.Value,
            createdOn
        );

        return Result.Ok(history);
    }
}





