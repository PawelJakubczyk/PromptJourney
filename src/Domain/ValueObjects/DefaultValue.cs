using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public sealed record DefaultValue : ValueObject<string>, ICreatable<DefaultValue, string?>
{
    public const int MaxLength = 50;
    public static readonly DefaultValue None = new(string.Empty);
    public override bool IsNone => this == None;

    private DefaultValue(string value) :
        base(value)
    { }

    public static Result<DefaultValue> Create(string? value)
    {

        if (string.IsNullOrWhiteSpace(value))
            return Result.Ok(None);

        value = value.Trim().ToLower();

        var result = WorkflowPipeline
            .Empty()
            .CongregateErrors(
                pipeline => pipeline.IfLengthTooLong<DefaultValue>(value!, MaxLength),
                pipeline => pipeline.IfContainsSuspiciousContent<DefaultValue>(value))
            .ExecuteIfNoErrors<DefaultValue>(() => new DefaultValue(value))
            .MapResult<DefaultValue>();

        return result;
    }
}
