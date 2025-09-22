using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;

namespace Domain.ValueObjects;

public record PropertyName : ValueObject<string>, ICreatable<PropertyName, string>
{
    public const int MaxLength = 25;

    private PropertyName(string value) : base(value) { }

    public static Result<PropertyName> Create(string value)
    {
        List<Error> errors = [];

        errors
            .IfNullOrWhitespace<DomainLayer, PropertyName>(value)
            .IfLengthTooLong<DomainLayer, PropertyName>(value, MaxLength);

        if (errors.Count != 0)
            return Result.Fail<PropertyName>(errors);

        return Result.Ok(new PropertyName(value));
    }
}