using FluentResults;
using System.Text.RegularExpressions;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.Errors;

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

    public static List<DomainError> IfLengthTooLong<TValue>(this List<DomainError> domainErrors, string value, int maxLength)
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

    public static List<DomainError> IfListIsEmpty<TValue>(this List<DomainError> domainErrors, List<TValue>? items)
    {
        if (items != null && items.Count == 0)
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: Cannot be an empty collection."));
        }
        return domainErrors;
    }

    public static List<DomainError> CollectErrors<T>(
        this List<DomainError> errors,
        Result<T>? result
    )
    {
        if (result is not null && result.IsFailed)
        {
            errors.AddRange(
                result.Errors.OfType<DomainError>()
            );
        }

        return errors;
    }
}