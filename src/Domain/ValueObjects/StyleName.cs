using Domain.Extensions;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

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
            .IfLenghtToLong<StyleName>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<StyleName>(errors);

        return Result.Ok(new StyleName(value));
    }
}