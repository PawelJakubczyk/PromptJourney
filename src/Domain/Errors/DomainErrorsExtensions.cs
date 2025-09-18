using Domain.Abstractions;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;

namespace Domain.Errors;

public static class DomainErrorsExtensions
{
    public static List<Error> IfNullOrWhitespace<TLayer, TValue>(this List<Error> Errors, string value)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: {value} cannot be null or whitespace."));
        }

        return Errors;
    }

    public static List<Error> IfWhitespace<TLayer, TValue>(this List<Error> Errors, string value)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (string.IsNullOrWhiteSpace(value) && value != null)
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: cannot be whitespace."));
        }

        return Errors;
    }

    public static List<Error> IfNull<TLayer, TValue>(this List<Error> Errors, object? value)
        where TLayer : ILayer
        where TValue : ValueObject<object>
    {
        if (value is null)
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: cannot be null."));
        }
        return Errors;
    }

    public static List<Error> IfNull<TLayer, TValue>(this List<Error> Errors, List<object?> value)
        where TLayer : ILayer
        where TValue : ValueObject<object>
    {
        if (value is null)
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: cannot be null."));
        }
        return Errors;
    }

    public static List<Error> IfLengthTooLong<TLayer, TValue>(this List<Error> Errors, string value, int maxLength)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (value?.Length > maxLength)
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: {value} cannot be longer than {maxLength} characters."));
        }

        return Errors;
    }

    public static List<Error> IfListIsEmpty<TLayer, TValue>(this List<Error> Errors, List<TValue>? items)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (items != null && items.Count == 0)
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: Cannot be an empty collection."));
        }
        return Errors;
    }

    public static List<Error> IfListNotContain<TLayer, TValue>(this List<Error> Errors, List<TValue>? items, TValue element)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (items != null && items.Contains(element) == false)
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: Collection does not contain the required element."));
        }
        return Errors;
    }

    public static List<Error> IfListContain<TLayer, TValue>(this List<Error> Errors, List<TValue>? items, TValue element)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (items != null && items.Contains(element))
        {
            Errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: Collection already contains the element."));
        }
        return Errors;
    }

    public static List<Error> CollectErrors<TLayer, TValue>(this List<Error> errors, Result<TValue>? result)
    {
        if (result is not null && result.IsFailed)
        {
            errors.AddRange(result.Errors.OfType<Error>());
        }

        return errors;
    }
}