using Domain.Abstractions;
using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed class MaxValue : IValueObject<MaxValue, string?>
{
    public const int MaxLength = 50;
    public string? Value { get; }

    private MaxValue(string? value)
    {
        Value = value;
    }

    public static Result<MaxValue> Create(string? value)
    {
        if (value == null)
            return Result.Ok(new MaxValue(null));

        List<DomainError> errors = [];

        errors
            .IfWhitespace<MaxValue>(value)
            .IfLengthTooLong<MaxValue>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<MaxValue>(errors);

        return Result.Ok(new MaxValue(value));
    }

    public override string? ToString() => Value;
}