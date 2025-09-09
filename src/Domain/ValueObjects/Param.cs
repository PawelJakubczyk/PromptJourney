using Domain.Abstractions;
using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed class Param : IValueObject<Param, string>
{
    public const int MaxLength = 100;
    public string Value { get; }

    private Param(string value)
    {
        Value = value;
    }

    public static Result<Param> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<Param>(value)
            .IfLengthTooLong<Param>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Param>(errors);

        return Result.Ok(new Param(value));
    }

    public override string ToString() => Value;
}