using Domain.Abstractions;
using Domain.Errors;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record Tag : ValueObject<string>, ICreatable<Tag, string>
{
    public const int MaxLength = 50;

    private Tag(string value) : base(value) { }

    public static Result<Tag> Create(string value)
    {
        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, Tag>(value)
            .IfLengthTooLong<DomainLayer, Tag>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Tag>(errors);

        return Result.Ok(new Tag(value));
    }
}