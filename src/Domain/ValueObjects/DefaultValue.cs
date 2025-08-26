using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class DefaultValue
{
    public const int MaxLength = 50;

    private readonly static List<DomainError> _errors = [];
    public string? Value { get; }

    private DefaultValue(string? value)
    {
        Value = value;
    }

    public static Result<DefaultValue> Create(string? value)
    {
        _errors.Clear();

        if (value == null)
            return Result.Ok(new DefaultValue(null));

        ValidateDefaultValueNotEmpty(value);
        ValidateDefaultValueLength(value);

        if (_errors.Any())
            return Result.Fail<DefaultValue>(_errors);

        return Result.Ok(new DefaultValue(value));
    }

    private static void ValidateDefaultValueNotEmpty(string value)
    {
        if (value.Length == 0)
        {
            _errors.Add(DefaultValueEmptyError);
        }
    }

    private static void ValidateDefaultValueLength(string value)
    {
        if (value.Length > MaxLength)
        {
            _errors.Add(DefaultValueTooLongError.WithDetail($"default value length: {value.Length}"));
        }
    }

    public override string? ToString() => Value ?? null;
}