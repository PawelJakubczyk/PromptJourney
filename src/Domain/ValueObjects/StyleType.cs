using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial class StyleType
{
    public const int MaxLength = 100;

    private readonly static List<DomainError> _errors = [];
    public string Value { get; }

    private StyleType(string value)
    {
        Value = value;
    }

    public static Result<StyleType> Create(string value)
    {
        _errors.Clear();

        ValidateTypeNotEmpty(value);
        ValidateTypeLength(value);
        ValidateTypeFormat(value);

        if (_errors.Any())
            return Result.Fail<StyleType>(_errors);

        return Result.Ok(new StyleType(value));
    }

    private static void ValidateTypeNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add(TypeNullOrEmptyError);
        }
    }

    private static void ValidateTypeLength(string value)
    {
        if (value?.Length > MaxLength)
        {
            _errors.Add(TypeToLongError.WithDetail($"type length: {value.Length}"));
        }
    }

    private static void ValidateTypeFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        // Type should be a valid category name
        if (!ValidTypeRegex().IsMatch(value))
        {
            _errors.Add(new DomainError("Type contains invalid characters. Only letters, numbers, spaces, and hyphens are allowed."));
        }
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-zA-Z0-9\s\-]+$")]
    private static partial Regex ValidTypeRegex();
}