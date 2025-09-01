using Domain.Errors;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed partial class ExampleLink
{
    public const int MaxLength = 200;
    public string Value { get; }

    private ExampleLink(string value)
    {
        Value = value;
    }

    public static Result<ExampleLink> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<ExampleLink>(value)
            .IfLengthTooLong<ExampleLink>(value, MaxLength)
            .IfLinkFormatInvalid(value);

        if (errors.Count != 0)
            return Result.Fail<ExampleLink>(errors);

        return Result.Ok(new ExampleLink(value));
    }

    public override string ToString() => Value;
    public Uri ToUri() => new(Value);
}