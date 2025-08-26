using FluentResults;
using static Domain.Errors.DomainErrorMessages;
using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public sealed partial class Tag
{
    public const int MaxLength = 50;

    private readonly static List<DomainError> _errors = [];
    public string Value { get; }

    private Tag(string value)
    {
        Value = value;
    }

    public static Result<Tag> Create(string value)
    {
        _errors.Clear();

        ValidateTagNotEmpty(value);
        ValidateTagLength(value);
        ValidateTagFormat(value);

        if (_errors.Any())
            return Result.Fail<Tag>(_errors);

        return Result.Ok(new Tag(value));
    }

    private static void ValidateTagNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add(new DomainError("Tag cannot be null or empty."));
        }
    }

    private static void ValidateTagLength(string value)
    {
        if (value?.Length > MaxLength)
        {
            _errors.Add(TagTooLongError.WithDetail($"tag length: {value.Length}"));
        }
    }

    private static void ValidateTagFormat(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return;

        // Tags should be simple, clean identifiers
        if (!ValidTagRegex().IsMatch(value))
        {
            _errors.Add(new DomainError("Tag contains invalid characters. Only letters, numbers, and hyphens are allowed."));
        }
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-zA-Z0-9\-]+$")]
    private static partial Regex ValidTagRegex();
}