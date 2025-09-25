using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record Description : ValueObject<string?>, ICreatable<Description, string?>
{
    public const int MaxLength = 500;

    private Description(string? value) : base(value) { }

    public static Result<Description> Create(string? value)
    {
        if (value == null)
            return Result.Ok(new Description(default(string)));

        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .IfWhitespace<DomainLayer, Description>(value)
                .IfLengthTooLong<DomainLayer, Description>(value, MaxLength))
            .ExecuteIfNoErrors<Description>(() => new Description(value))
            .MapResult(d => d);

        return result;
    }
}
