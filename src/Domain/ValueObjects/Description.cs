using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class Description
{
    public const int MaxLength = 500;

    private readonly static List<DomainError> _errors = [];
    public string? Value { get; }

    private Description(string? value)
    {
        Value = value;
    }

    public static Result<Description> Create(string? value)
    {
        _errors.Clear();

        if (value == null)
            return Result.Ok(new Description(null));

        ValidateDescriptionNotEmpty(value);
        ValidateDescriptionLength(value);

        if (_errors.Any())
            return Result.Fail<Description>(_errors);

        return Result.Ok(new Description(value));
    }

    private static void ValidateDescriptionNotEmpty(string value)
    {
        if (value.Length == 0)
        {
            _errors.Add(DescriptionEmptyError);
        }
    }

    private static void ValidateDescriptionLength(string value)
    {
        if (value.Length > MaxLength)
        {
            _errors.Add(DescriptionToLongError.WithDetail($"description length: {value.Length}"));
        }
    }

    public override string ToString() => Value ?? string.Empty;
}