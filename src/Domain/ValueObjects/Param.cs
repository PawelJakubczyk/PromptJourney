using Domain.Extensions;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class Param
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
            .IfLenghtToLong<Param>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Param>(errors);

        return Result.Ok(new Param(value));
    }

    public override string ToString() => Value;
}