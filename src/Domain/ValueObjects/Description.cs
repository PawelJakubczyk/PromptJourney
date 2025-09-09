using Domain.Abstractions;
using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed class Description : IValueObject<Description, string>
{
    public const int MaxLength = 500;
    public string Value { get; }

    private Description(string value)
    {
        Value = value;
    }

    public static Result<Description> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfWhitespace<Description>(value)
            .IfLengthTooLong<Description>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Description>(errors);

        return Result.Ok(new Description(value));
    }

    public override string ToString() => Value ?? string.Empty;
}