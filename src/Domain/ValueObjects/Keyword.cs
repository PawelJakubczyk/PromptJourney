using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public class Keyword
{
    public const int MaxLength = 50;
    public string Value { get; }

    private Keyword(string value)
    {
        Value = value;
    }

    public static Result<Keyword> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<Keyword>(value)
            .IfLengthTooLong<Keyword>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Keyword>(errors);

        return Result.Ok(new Keyword(value));
    }

    public override string ToString() => Value;
}
