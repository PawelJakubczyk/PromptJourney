using Domain.Entities.MidjourneyStyles;
using Domain.Entities.MidjourneyVersions;
using Domain.Entities.MidjourneyVersions.Exceptions;
using Domain.ErrorMassages;
using Domain.Exceptions;
using FluentResults;

namespace Domain.Entities.MidjourneyPromtHistory;

public class MidjourneyPromptHistory
{
    // Columns
    public Guid HistoryId { get; set; }
    public string Prompt { get; set; }
    public string Version { get; set; }
    public DateTime CreatedOn { get; set; }

    // Navigation
    public MidjourneyVersionsMaster VersionMaster { get; set; }
    public List<MidjourneyStyle> MidjourneyStyles { get; set; } = [];

    // Errors
    private static List<MidjourneyEntitiesException> _errors = [];

    // Constructor
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

    private MidjourneyPromptHistory()
    {

    }

    public static Result<MidjourneyPromptHistory> Create
    (
        string prompt,
        string version
    )
    {
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

    private static void ValidatePrompt(string prompt)
    {
        if (string.IsNullOrEmpty(prompt))
            _errors.Add(new VersionValidationException("Prompt", ErrorMessages.PromptNullOrEmpty));
        else if (prompt.Length > 1000)
            _errors.Add(new VersionValidationException("Prompt", ErrorMessages.PromptTooLong));
    }

    private static void ValidateVersion(string? version)
    {
        if (string.IsNullOrEmpty(version))
            _errors.Add(new VersionValidationException("Version", ErrorMessages.VersionNullOrEmpty));
        else if (version.Length > 10)
            _errors.Add(new VersionValidationException("Version", ErrorMessages.VersionTooLong));
    }
}
