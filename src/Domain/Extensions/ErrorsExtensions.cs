using System.Text.RegularExpressions;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.Extensions;

public static class ErrorsExtensions
{
    public static List<DomainError> If(this List<DomainError> domainErrors, bool condition, DomainError errorIfTrue)
    {
        if (condition)
        {
            domainErrors.Add(errorIfTrue);
        }

        return domainErrors;
    }

    public static List<DomainError> IfNullOrWhitespace<TValue>(this List<DomainError> domainErrors, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: {value} cannot be null or whitespace."));
        }

        return domainErrors;
    }

    public static List<DomainError> IfWhitespace<TValue>(this List<DomainError> domainErrors, string value)
    {
        if (value == "")
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: {value} cannot be whitespace."));
        }

        return domainErrors;
    }

    public static List<DomainError> IfLenghtToLong<TValue>(this List<DomainError> domainErrors, string value, int maxLength)
    {
        if (value?.Length > maxLength)
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: {value} cannot be longer than {maxLength} characters."));
        }

        return domainErrors;
    }

    public static List<DomainError> IfLinkFormatInvalid(this List<DomainError> domainErrors, string value)
    {
        var isValid = Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (isValid)
        {
            domainErrors.Add(new DomainError($"Invalid URL format: {value}"));
        };

        return domainErrors;
    }

    public static List<DomainError> IfVersionFormatInvalid(this List<DomainError> domainErrors, string value)
    {
        bool isValidNumeric = Regex.IsMatch(value, @"^[1-9](\.[0-9])?$");
        bool isValidNiji = Regex.IsMatch(value, @"^niji [4-6]$");

        if (!isValidNumeric && !isValidNiji)
        {
            domainErrors.Add(new DomainError
            (
                $"Invalid version format: {value}. Expected numeric (e.g., '5', '5.1') or niji format (e.g., 'niji 5')"
            ));
        }

        return domainErrors;
    }

    public static List<DomainError> IfTooManyItems<TValue>(this List<DomainError> domainErrors, TValue[]? items, int maxItems)
    {
        if (items != null && items.Length > maxItems)
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: Cannot have more than {maxItems} items."));
        }
        return domainErrors;
    }

    public static List<DomainError> IfEmptyItems<TValue>(this List<DomainError> domainErrors, TValue[]? items)
    {
        if (items != null && items.Length == 0)
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: Cannot be an empty collection."));
        }
        return domainErrors;
    }


}