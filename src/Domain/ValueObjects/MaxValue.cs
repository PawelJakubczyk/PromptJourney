using Domain.Abstractions;
using Domain.Extensions;
using FluentResults;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record MaxValue : ValueObject<string?>, ICreatable<MaxValue, string?>
{
    public const int MaxLength = 50;

    private MaxValue(string? value) : base(value) { }

    public static Result<MaxValue> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .IfWhitespace<DomainLayer, MaxValue>(value)
            .IfLengthTooLong<DomainLayer, MaxValue>(value, MaxLength)
            .ExecuteIfNoErrors<MaxValue>(() => new MaxValue(value))
            .MapResult<MaxValue>();

        return result;
    }
}