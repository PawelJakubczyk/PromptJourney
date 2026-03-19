using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record MaxValue : ValueObject<string>, ICreatable<MaxValue, string?>
{
    public const int MaxLength = 12;
    public static readonly MaxValue None = new(string.Empty);
    public override bool IsNone => this == None;

    private MaxValue(string value) : base(value) { }

    public static Result<MaxValue> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Ok(None);

        value = value.Trim();

        var result = WorkflowPipeline
            .Empty()
            .CongregateErrors(
                pipeline => pipeline.IfLengthTooLong<MaxValue>(value, MaxLength),
                pipeline => pipeline.IfContainsSuspiciousContent<MaxValue>(value))
            .ExecuteIfNoErrors<MaxValue>(() => new MaxValue(value))
            .MapResult<MaxValue>();

        return result;
    }
}