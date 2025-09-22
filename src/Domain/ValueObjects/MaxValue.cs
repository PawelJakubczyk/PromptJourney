using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record MaxValue : ValueObject<string?>, ICreatable<MaxValue, string?>
{
    public const int MaxLength = 50;

    private MaxValue(string? value) : base(value) { }

    public static Result<MaxValue> Create(string? value)
    {
        if (value == null)
            return Result.Ok(new MaxValue(default(string)));

        List<Error> errors = [];

        errors
            .IfWhitespace<DomainLayer, MaxValue>(value)
            .IfLengthTooLong<DomainLayer, MaxValue>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<MaxValue>(errors);

        return Result.Ok(new MaxValue(value));
    }
}