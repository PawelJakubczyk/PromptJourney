using Domain.Abstractions;
using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed class MinValue : IValueObject<MinValue, string?>
{
    public const int MaxLength = 50;
    public string? Value { get; }

    private MinValue(string? value)
    {
        Value = value;
    }

    public static Result<MinValue> Create(string? value)
    {
        if (value == null)
            return Result.Ok(new MinValue(null));

        List<DomainError> errors = [];

        errors
            .IfWhitespace<MinValue>(value)
            .IfLengthTooLong<MinValue>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<MinValue>(errors);

        return Result.Ok(new MinValue(value));
    }

    public override string? ToString() => Value;
}