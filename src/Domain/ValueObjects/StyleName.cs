using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial class StyleName
{
    public const int MaxLength = 150;

    private readonly static List<DomainError> _errors = [];
    public string Value { get; }

    private StyleName(string value)
    {
        Value = value;
    }

    public static Result<StyleName> Create(string value)
    {
        _errors.Clear();

        ValidateStyleNameNotEmpty(value);
        ValidateStyleNameLength(value);
        ValidateStyleNameFormat(value);

        if (_errors.Any())
            return Result.Fail<StyleName>(_errors);

        return Result.Ok(new StyleName(value));
    }

    private static void ValidateStyleNameNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add(NameNullOrEmptyError);
        }
    }

    private static void ValidateStyleNameLength(string value)
    {
        if (value?.Length > MaxLength)
        {
            _errors.Add(NameToLongError.WithDetail($"style name length: {value.Length}"));
        }
    }

    private static void ValidateStyleNameFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        // Style name should not contain special characters that could cause issues
        if (ContainsInvalidCharacters(value))
        {
            _errors.Add(new DomainError("Style name contains invalid characters. Only letters, numbers, spaces, hyphens, and underscores are allowed."));
        }
    }

    private static bool ContainsInvalidCharacters(string value)
    {
        return !ValidStyleNameRegex().IsMatch(value);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-zA-Z0-9\s\-_]+$")]
    private static partial Regex ValidStyleNameRegex();
}