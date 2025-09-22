using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record MinValue : ValueObject<string?>, ICreatable<MinValue, string?>
{
    public const int MaxLength = 50;

    private MinValue(string? value) : base(value) { }

    public static Result<MinValue> Create(string? value)
    {
        if (value == null)
            return Result.Ok(new MinValue(default(string)));

        List<Error> errors = [];

        errors
            .IfWhitespace<DomainLayer, MinValue>(value)
            .IfLengthTooLong<DomainLayer, MinValue>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<MinValue>(errors);

        return Result.Ok(new MinValue(value));
    }
}