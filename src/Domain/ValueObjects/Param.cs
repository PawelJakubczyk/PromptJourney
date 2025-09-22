using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record Param : ValueObject<string>, ICreatable<Param, string>
{
    public const int MaxLength = 100;

    private Param(string value) : base(value) { }

    public static Result<Param> Create(string value)
    {
        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, Param>(value)
            .IfLengthTooLong<DomainLayer, Param>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Param>(errors);

        return Result.Ok(new Param(value));
    }
}