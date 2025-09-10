using FluentResults;

namespace Domain.Errors;

public static class DomainErrorsExtensions
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
        if (string.IsNullOrWhiteSpace(value) && value != null)
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: cannot be whitespace."));
        }

        return domainErrors;
    }

    public static List<DomainError> IfNull<TValue>(this List<DomainError> domainErrors, object? value)
    {
        if (value is null)
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: cannot be null."));
        }
        return domainErrors;
    }

    public static List<DomainError> IfLengthTooLong<TValue>(this List<DomainError> domainErrors, string value, int maxLength)
    {
        if (value?.Length > maxLength)
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: cannot be longer than {maxLength} characters."));
        }

        return domainErrors;
    }

    public static List<DomainError> IfLinkFormatInvalid(this List<DomainError> domainErrors, string value)
    {
        var isValid = Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (!isValid)
        {
            domainErrors.Add(new DomainError($"Invalid URL format: {value}"));
        }

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

    public static List<DomainError> IfDoesNotContain<TValue>(this List<DomainError> domainErrors, List<TValue>? items, TValue element)
    {
        if (items != null && items.Contains(element) == false)
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: Collection does not contain the required element."));
        }
        return domainErrors;
    }

    public static List<DomainError> IfContain<TValue>(this List<DomainError> domainErrors, List<TValue>? items, TValue element)
    {
        if (items != null && items.Contains(element))
        {
            domainErrors.Add(new DomainError($"{nameof(TValue)}: Collection already contains the element."));
        }
        return domainErrors;
    }

    public static List<DomainError> CollectErrors<T>(
        this List<DomainError> errors,
        Result<T>? result)
    {
        if (result is not null && result.IsFailed)
        {
            errors.AddRange(result.Errors.OfType<DomainError>());
        }

        return errors;
    }

}