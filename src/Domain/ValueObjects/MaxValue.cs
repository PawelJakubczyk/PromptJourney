using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed class MaxValue
{
    public const int MaxLength = 50;
    public string? Value { get; }

    private MaxValue(string? value)
    {
        Value = value;
    }

    public static Result<MaxValue> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfWhitespace<MaxValue>(value)
            .IfLengthTooLong<MaxValue>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<MaxValue>(errors);

        return Result.Ok(new MaxValue(value));
    }

    public override string ToString() => Value ?? string.Empty;
}