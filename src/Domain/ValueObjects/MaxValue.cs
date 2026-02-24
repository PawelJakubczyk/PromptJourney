using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Constants;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record MaxValue : ValueObject<string?>, ICreatable<MaxValue, string?>
{
    public const int MaxLength = 50;

    private MaxValue(string? value) : base(value) { }

    public static Result<MaxValue> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            value = null;

        var result = WorkflowPipeline
            .Empty()
            .IfLengthTooLong<DomainLayer, MaxValue>(value, MaxLength)
            .ExecuteIfNoErrors<MaxValue>(() => new MaxValue(value))
            .MapResult<MaxValue>();

        return result;
    }
}