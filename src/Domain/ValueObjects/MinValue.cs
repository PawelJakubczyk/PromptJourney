using Domain.Abstractions;
using Utilities.Workflows;
using Utilities.Results;
using Domain.Extensions;

namespace Domain.ValueObjects;

public record MinValue : ValueObject<string>, ICreatable<MinValue, string>
{
    public const int MaxLength = 50;
    public static readonly MinValue None = new(string.Empty);
    public override bool IsNone => this == None;

    private MinValue(string value) : base(value) { }

    public static Result<MinValue> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Ok(None);

        value = value.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfLengthTooLong<MinValue>(value, MaxLength)
            .ExecuteIfNoErrors<MinValue>(() => new MinValue(value))
            .MapResult<MinValue>();

        return result;
    }
}