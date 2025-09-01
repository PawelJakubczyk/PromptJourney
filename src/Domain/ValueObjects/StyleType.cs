using Domain.Errors;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed partial class StyleType
{
    public const int MaxLength = 100;
    public string Value { get; }

    private StyleType(string value)
    {
        Value = value;
    }

    public static Result<StyleType> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<StyleType>(value)
            .IfLengthTooLong<StyleType>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<StyleType>(errors);

        return Result.Ok(new StyleType(value));
    }

    public override string ToString() => Value;
}