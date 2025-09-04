using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed partial class StyleName
{
    public const int MaxLength = 150;
    public string Value { get; }

    private StyleName(string value)
    {
        Value = value;
    }

    public static Result<StyleName> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<StyleName>(value)
            .IfLengthTooLong<StyleName>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<StyleName>(errors);

        return Result.Ok(new StyleName(value));
    }
}