using Domain.Abstractions;
using Domain.Errors;
using FluentResults;


namespace Domain.ValueObjects;

public sealed partial class StyleReferenceName : IValueObject<StyleReferenceName, string>
{
    public const int MaxLength = 150;
    public string Value { get; }

    private StyleReferenceName(string value)
    {
        Value = value;
    }

    public static Result<StyleReferenceName> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<StyleReferenceName>(value)
            .IfLengthTooLong<StyleReferenceName>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<StyleReferenceName>(errors);

        return Result.Ok(new StyleReferenceName(value));
    }
}