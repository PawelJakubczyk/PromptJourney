using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record DefaultValue : ValueObject<string?>, ICreatable<DefaultValue, string?>
{
    public const int MaxLength = 50;

    private DefaultValue(string? value) : base(value) { }

    public static Result<DefaultValue> Create(string? value)
    {
        if (value == null)
            return Result.Ok(new DefaultValue(default(string)));

        List<Error> errors = [];

        errors
            .IfWhitespace<DomainLayer, DefaultValue>(value)
            .IfLengthTooLong<DomainLayer, DefaultValue>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<DefaultValue>(errors);

        return Result.Ok(new DefaultValue(value));
    }
}