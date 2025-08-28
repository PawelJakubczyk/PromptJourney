using Domain.Extensions;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class DefaultValue
{
    public const int MaxLength = 50;
    public string? Value { get; }

    private DefaultValue(string? value)
    {
        Value = value;
    }

    public static Result<DefaultValue> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfWhitespace<DefaultValue>(value)
            .IfLenghtToLong<DefaultValue>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<DefaultValue>(errors);

        return Result.Ok(new DefaultValue(value));
    }

    public override string ToString() => Value ?? string.Empty;
}