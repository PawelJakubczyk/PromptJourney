using Domain.Abstractions;
using FluentResults;
using Utilities.Constants;
using Utilities.Errors;

namespace Domain.Extensions;

public static class ErrorsExtensions
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

    public static List<Error> IfListHasDuplicates<TLayer, TValue>(this List<Error> errors, List<TValue>? values)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (values is null)
        {
            return errors;
        }

        var duplicates = values
            .GroupBy(value => value)
            .Where(grouping => grouping.Count() > 1)
            .Select(grouping => grouping.Key)
            .ToList();

        if (duplicates.Count != 0)
        {
            var duplicateNames = string.Join(", ", duplicates.Select(d => d.ToString()));
            errors.Add(new Error<TLayer>($"{typeof(TValue).Name}: contains duplicates -> {duplicateNames}."));
        }

        return errors;
    }

    public static List<Error> CollectErrors<TValue>(this List<Error> errors, Result<TValue>? result)
            where TValue : ValueObject<string>
    {
        if (result is not null && result.IsFailed)
        {
            errors.AddRange(result.Errors.OfType<Error>());
        }

        return errors;
    }

    public static List<Error> CollectErrors<TValue>(this List<Error> errors, List<Result<TValue>>? resultList)
            where TValue : ValueObject<string>
    {
        if (resultList != null && resultList.Count > 0)
        {

            foreach (var result in resultList)
            {
                errors.CollectErrors<TValue>(result);

                if (result is not null && result.IsFailed)
                {
                    errors.AddRange(result.Errors.OfType<Error>());
                }
            }
        }

        return errors;
    }
}