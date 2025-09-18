using Domain.Abstractions;
using Domain.Errors;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record Prompt : ValueObject<string>, ICreatable<Prompt, string>
{
    public const int MaxLength = 1000;

    private Prompt(string value) : base(value) { }

    public static Result<Prompt> Create(string value)
    {

        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, Prompt>(value)
            .IfLengthTooLong<DomainLayer, Prompt>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Prompt>(errors);

        return Result.Ok(new Prompt(value));
    }
}