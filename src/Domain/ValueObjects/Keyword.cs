using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Keyword : ValueObject<string>, ICreatable<Keyword, string?>
{
    public const int MaxLength = 50;
    public override bool IsNone => false;

    private Keyword(string value) : base(value) { }

    public static Result<Keyword> Create(string? value)
    {
        value = value?.Trim().ToLower();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<Keyword>(value)
            .CongregateErrors(
                pipeline => pipeline.IfLengthTooLong<Keyword>(value!, MaxLength),
                pipeline => pipeline.IfContainsSuspiciousContent<Keyword>(value!))
            .ExecuteIfNoErrors<Keyword>(() => new Keyword(value!))
            .MapResult<Keyword>();

        return result;
    }
}
