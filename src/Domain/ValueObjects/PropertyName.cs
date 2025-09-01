using Domain.Errors;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class PropertyName
{
    public const int MaxLength = 25;
    public string Value { get; }

    private PropertyName(string value)
    {
        Value = value;
    }

    public static Result<PropertyName> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<PropertyName>(value)
            .IfLengthTooLong<PropertyName>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<PropertyName>(errors);

        return Result.Ok(new PropertyName(value));
    }

    public override string ToString() => Value;
}