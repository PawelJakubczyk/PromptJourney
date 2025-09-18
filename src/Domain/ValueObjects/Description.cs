using Domain.Abstractions;
using Domain.Errors;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record Description : ValueObject<string?>, ICreatable<Description, string?>
{
    public const int MaxLength = 500;

    private Description(string? value) : base(value) { }

    public static Result<Description> Create(string? value)
    {
        if (value == null)
            return Result.Ok(new Description(default(string)));

        List<Error> errors = [];

        errors
            .IfWhitespace<DomainLayer, Description>(value)
            .IfLengthTooLong<DomainLayer, Description>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Description>(errors);

        return Result.Ok(new Description(value));
    }
}