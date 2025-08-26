using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class Param
{
    public const int MaxLength = 100;

    private readonly static List<DomainError> _errors = [];
    public string Value { get; }

    private Param(string value)
    {
        Value = value;
    }

    public static Result<Param> Create(string value)
    {
        _errors.Clear();

        ValidateParameterNotEmpty(value);
        ValidateParameterLength(value);

        if (_errors.Any())
            return Result.Fail<Param>(_errors);

        return Result.Ok(new Param(value));
    }

    private static void ValidateParameterNotEmpty(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            _errors.Add(ParameterNullOrEmptyError);
        }
    }

    private static void ValidateParameterLength(string value)
    {
        if (value?.Length > MaxLength)
        {
            _errors.Add(ParameterTooLongError.WithDetail($"parameter: '{value}' (length: {value.Length})"));
        }
    }

    public override string ToString() => Value;
}