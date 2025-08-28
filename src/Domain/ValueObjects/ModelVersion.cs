using Domain.Extensions;
using FluentResults;
using static Domain.Errors.DomainErrorMessages;

namespace Domain.ValueObjects;

public sealed class ModelVersion
{
    public const int MaxLength = 10;
    public string Value { get; }

    private ModelVersion(string value)
    {
        Value = value;
    }

    public static Result<ModelVersion> Create(string value)
    {
        List<DomainError> errors = [];

        errors
            .IfNullOrWhitespace<ModelVersion>(value)
            .IfLenghtToLong<ModelVersion>(value, MaxLength)
            .IfVersionFormatInvalid(value);

        if (errors.Count != 0)
            return Result.Fail<ModelVersion>(errors);

        return Result.Ok(new ModelVersion(value));
    }

    public override string ToString() => Value;
};

