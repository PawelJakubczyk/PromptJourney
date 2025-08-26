using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class MaxValue
{
    public const int MaxLength = 50;

    private readonly static List<DomainError> _errors = [];
    public string? Value { get; }

    private MaxValue(string? value)
    {
        Value = value;
    }

    public static Result<MaxValue> Create(string? value)
    {
        _errors.Clear();

        if (value == null)
            return Result.Ok(new MaxValue(null));

        ValidateMaxValueNotEmpty(value);
        ValidateMaxValueLength(value);

        if (_errors.Any())
            return Result.Fail<MaxValue>(_errors);

        return Result.Ok(new MaxValue(value));
    }

    private static void ValidateMaxValueNotEmpty(string value)
    {
        if (value.Length == 0)
        {
            _errors.Add(MaxValueEmptyError);
        }
    }

    private static void ValidateMaxValueLength(string value)
    {
        if (value.Length > MaxLength)
        {
            _errors.Add(MaxValueTooLongError.WithDetail($"max value length: {value.Length}"));
        }
    }

    public override string ToString() => Value ?? string.Empty;
}