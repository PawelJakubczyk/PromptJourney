using Domain.Errors;
using FluentResults;

namespace Domain.ValueObjects;

public sealed partial class Tag
{
    public const int MaxLength = 50;
    public string Value { get; }

    private Tag(string value)
    {
        Value = value;
    }

    public static Result<Tag> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<Tag>(value)
            .IfLengthTooLong<Tag>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<Tag>(errors);

        return Result.Ok(new Tag(value));
    }

    public override string ToString() => Value;
}