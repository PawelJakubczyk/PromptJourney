using FluentResults;

namespace Domain.Errors;

public static class DomainErrorMessages
{
    public class DomainError : Error
    {
        public DomainError(string message)
            : base(message)
        {
        }

        public DomainError WithDetail(string detail)
        {
            return new DomainError($"{Message} {CutOffDetail(detail)}");
        }

        private static string CutOffDetail(string detail, int maxLength = 40)
        {
            return detail.Length > maxLength ? detail[..maxLength] : detail;
        }
    }

    // # Common errors used across multiple entities #

    // Description errors
    public static readonly DomainError DescriptionEmptyError = new($"Description cannot be empty.");
    public static readonly DomainError DescriptionToLongError = new("Description is too long.");
    public static readonly DomainError ParameterNullOrEmptyError = new("Parameter value cannot be null or empty.");
    public static readonly DomainError ParameterTooLongError = new("Parameter value is too long. It must not exceed 100 characters.");

    // Version errors
    public static readonly DomainError VersionNullOrEmptyError = new("Version cannot be null or empty.");
    public static readonly DomainError VersionToLongError = new("Version is too long.");
    public static readonly DomainError VersionNotFoundError = new("Version not found.");

    // # Style-specific errors #

    // Name errors
    public static readonly DomainError NameNullOrEmptyError = new("Name cannot be null or empty.");
    public static readonly DomainError NameToLongError = new("Name is too long.");
        
    // Type errors
    public static readonly DomainError TypeNullOrEmptyError = new("Type cannot be null or empty.");
    public static readonly DomainError TypeToLongError = new("Type is too long.");
        
    // Tag errors
    public static readonly DomainError TagTooLongError = new("Tag is too long. It must not exceed 50 characters.");
    public static readonly DomainError TagsTooManyError = new("Tags cannot exceed 10 items.");
    public static readonly DomainError TagNotFoundError = new("Tag not found in the collection.");
    public static readonly DomainError TagDuplicateError = new("Tag already exists in the collection.");
        
    // Example link errors
    public static readonly DomainError ExampleLinkTooLongError = new("Example link is too long. It must not exceed 200 characters.");
    public static readonly DomainError ExampleLinksTooManyError = new("Example links cannot exceed 10 items.");
    public static readonly DomainError ExampleLinkNotFoundError = new("Example link not found in the collection.");
    public static readonly DomainError ExampleLinkDuplicateError = new("Example link already exists in the collection.");


    // # Prompt history-specific errors #

    public static readonly DomainError PromptNullOrEmptyError = new("Prompt cannot be null or empty.");
    public static readonly DomainError PromptToLongError = new("Prompt is too long.");

    // # Version base-specific errors

    // Property name errors
    public static readonly DomainError PropertyNameNullOrEmptyError = new("Property name cannot be null or empty.");
    public static readonly DomainError PropertyNameTooLongError = new("Property name is too long. It must not exceed 25 characters.");
        
    // Parameters errors
    public static readonly DomainError ParametersEmptyError = new("Parameters collection cannot be empty.");
    public static readonly DomainError ParametersTooManyError = new("Parameters collection cannot exceed 10 items.");
    public static readonly DomainError ParameterNotFoundError = new("Parameter not found in specified version.");
        
    // Value-related errors
    public static readonly DomainError DefaultValueEmptyError = new("Default value cannot be empty.");
    public static readonly DomainError DefaultValueTooLongError = new("Default value is too long. It must not exceed 50 characters.");
    public static readonly DomainError MinValueEmptyError = new("Min value cannot be empty.");
    public static readonly DomainError MinValueTooLongError = new("Min value is too long. It must not exceed 50 characters.");
    public static readonly DomainError MaxValueEmptyError = new("Max value cannot be empty.");
    public static readonly DomainError MaxValueTooLongError = new("Max value is too long. It must not exceed 50 characters.");

    // # Version master-specific errors #
    public static readonly DomainError ReleaseDateInFutureError = new("Release date cannot be in the future.");
}

