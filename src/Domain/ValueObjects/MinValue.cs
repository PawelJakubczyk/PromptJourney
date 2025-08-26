using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class MinValue
{
    public const int MaxLength = 50;

    private readonly static List<DomainError> _errors = [];
    public string? Value { get; }

    private MinValue(string? value)
    {
        Value = value;
    }

    public static Result<MinValue> Create(string? value)
    {
        _errors.Clear();

        if (value == null)
            return Result.Ok(new MinValue(null));

        ValidateMinValueNotEmpty(value);
        ValidateMinValueLength(value);

        if (_errors.Any())
            return Result.Fail<MinValue>(_errors);

        return Result.Ok(new MinValue(value));
    }

    private static void ValidateMinValueNotEmpty(string value)
    {
        if (value.Length == 0)
        {
            _errors.Add(MinValueEmptyError);
        }
    }

    private static void ValidateMinValueLength(string value)
    {
        if (value.Length > MaxLength)
        {
            _errors.Add(MinValueTooLongError.WithDetail($"min value length: {value.Length}"));
        }
    }

    public override string ToString() => Value ?? string.Empty;
}