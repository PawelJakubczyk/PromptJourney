using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Validation;

namespace Domain.ValueObjects;

public record MaxValue : ValueObject<string?>, ICreatable<MaxValue, string?>
{
    public const int MaxLength = 50;

    private MaxValue(string? value) : base(value) { }

    public static Result<MaxValue> Create(string? value)
    {
        if (value is null)
            return Result.Ok(new MaxValue(default(string)));

        var result = WorkflowPipeline
            .Empty()
            .Validate(pipeline => pipeline
                .IfWhitespace<DomainLayer, MaxValue>(value)
                .IfLengthTooLong<DomainLayer, MaxValue>(value, MaxLength))
            .ExecuteIfNoErrors<MaxValue>(() => new MaxValue(value))
            .MapResult(v => v);

        return result;
    }
}
