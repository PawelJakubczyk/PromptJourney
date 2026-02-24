using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Constants;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.ValueObjects;

public record MinValue : ValueObject<string?>, ICreatable<MinValue, string?>
{
    public const int MaxLength = 50;

    private MinValue(string? value) : base(value) { }

    public static Result<MinValue> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            value = null;

        var result = WorkflowPipeline
            .Empty()
            .IfLengthTooLong<DomainLayer, MinValue>(value, MaxLength)
            .ExecuteIfNoErrors<MinValue>(() => new MinValue(value))
            .MapResult<MinValue>();

        return result;
    }
}