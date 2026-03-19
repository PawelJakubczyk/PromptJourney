using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.ValueObjects;

public record Tag : ValueObject<string>, ICreatable<Tag, string?>
{
    public const int MaxLength = 50;
    public override bool IsNone => false;

    private Tag(string value) : base(value) { }

    public static Result<Tag> Create(string? value)
    {
        value = value?.Trim().ToLower();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<Tag>(value)
            .CongregateErrors(
                pipeline => pipeline.IfLengthTooLong<Tag>(value!, MaxLength),
                pipeline => pipeline.IfContainsSuspiciousContent<Tag>(value!))
            .ExecuteIfNoErrors<Tag>(() => new Tag(value!))
            .MapResult<Tag>();

        return result;
    }
}
