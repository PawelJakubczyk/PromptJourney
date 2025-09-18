using Domain.Abstractions;
using Domain.Errors;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record StyleName : ValueObject<string>, ICreatable<StyleName, string>
{
    public const int MaxLength = 150;

    private StyleName(string value) : base(value) { }

    public static Result<StyleName> Create(string value)
    {
        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, StyleName>(value)
            .IfLengthTooLong<DomainLayer, StyleName>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<StyleName>(errors);

        return Result.Ok(new StyleName(value));
    }
}