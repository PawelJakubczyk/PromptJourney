using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record Keyword : ValueObject<string>, ICreatable<Keyword, string>
{
    public const int MaxLength = 50;

    private Keyword(string value) : base(value) { }

    public static Result<Keyword> Create(string? value)
    {
        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, Keyword>(value)
            .IfLengthTooLong<DomainLayer, Keyword>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Keyword>(errors);

        return Result.Ok(new Keyword(value));
    }
}
