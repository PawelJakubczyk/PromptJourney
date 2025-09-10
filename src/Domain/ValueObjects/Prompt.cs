using Domain.Abstractions;
using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed class Prompt : IValueObject<Prompt, string>
{
    public const int MaxLength = 1000;
    public string Value { get; }

    private Prompt(string value)
    {
        Value = value;
    }

    public static Result<Prompt> Create(string value)
    {

        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<Prompt>(value)
            .IfLengthTooLong<Prompt>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Prompt>(errors);

        return Result.Ok(new Prompt(value));
    }

    public override string ToString() => Value;
}