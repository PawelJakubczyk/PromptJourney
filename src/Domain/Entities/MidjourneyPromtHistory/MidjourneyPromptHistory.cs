using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.Entities.MidjourneyPromtHistory;

public class MidjourneyPromptHistory
{
    // Columns
    public Guid HistoryId { get; }
    public string Prompt { get; }
    public string Version { get; }
    public DateTime CreatedOn { get; }

    // Navigation
    public MidjourneyVersionsMaster VersionMaster { get; set; }
    public List<MidjourneyStyle> MidjourneyStyles { get; set; } = [];

    // Errors
    private static List<DomainError> _errors = [];

    // Constructors
    private MidjourneyPromptHistory()
    {
        // Parameterless constructor for EF Core
    }

    private MidjourneyPromptHistory
    (
        string prompt,
        string version
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
        string prompt,
        string version
    )
    {
        _errors.Clear();

        ValidatePrompt(prompt);
        ValidateVersion(version);

        if (_errors.Count > 0)
        {
            return Result.Fail<MidjourneyPromptHistory>(_errors.Select(e => e.Message));
        }

        var history = new MidjourneyPromptHistory
        (
            version,
            version
        );

        return Result.Ok(history);
    }

    // Validation methods
    private static void ValidatePrompt(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
            _errors.Add(PromptNullOrEmptyError);
        else if (prompt.Length > 1000)
            _errors.Add(PromptToLongError.WithDetail($"prompt: {prompt}."));
    }

    private static void ValidateVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
            _errors.Add(VersionNullOrEmptyError);
        else if (version.Length > 10)
            _errors.Add(VersionToLongError.WithDetail($"version: {version}."));
    }
}





