using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial class PropertyName
{
    public const int MaxLength = 25;

    private readonly static List<DomainError> _errors = [];
    public string Value { get; }

    private PropertyName(string value)
    {
        Value = value;
    }

    public static Result<PropertyName> Create(string value)
    {
        _errors.Clear();

        ValidatePropertyNameNotEmpty(value);
        ValidatePropertyNameLength(value);
        ValidatePropertyNameFormat(value);

        if (_errors.Any())
            return Result.Fail<PropertyName>(_errors);

        return Result.Ok(new PropertyName(value));
    }

    private static void ValidatePropertyNameNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add(PropertyNameNullOrEmptyError);
        }
    }

    private static void ValidatePropertyNameLength(string value)
    {
        if (value?.Length > MaxLength)
        {
            _errors.Add(PropertyNameTooLongError.WithDetail($"property name length: {value.Length}"));
        }
    }

    private static void ValidatePropertyNameFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        // Property name should be valid identifier-like (for Midjourney parameters like "aspect", "chaos", etc.)
        if (!ValidPropertyNameRegex().IsMatch(value))
        {
            _errors.Add(new DomainError($"Property name '{value}' contains invalid characters. Only letters, numbers, and underscores are allowed, and it must start with a letter or underscore."));
        }
    }

    [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*$")]
    private static partial Regex ValidPropertyNameRegex();

    [GeneratedRegex(@"^[a-zA-Z0-9\s\-_]+$")]
    private static partial Regex ValidStyleNameRegex();
}