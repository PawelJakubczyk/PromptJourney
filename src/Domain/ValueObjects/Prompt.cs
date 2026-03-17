using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.ValueObjects;

public record Prompt : ValueObject<string>, ICreatable<Prompt, string>
{
    public const int MaxLength = 1000;
    public static readonly Prompt None = new(string.Empty);
    public override bool IsNone => this == None;

    private Prompt(string value) : base(value) { }

    public static Result<Prompt> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Ok(None);

        value = value.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfLengthTooLong<Prompt>(value, MaxLength)
            .IfContainsSuspiciousContent<Prompt>(value)
            .ExecuteIfNoErrors<Prompt>(() => new Prompt(value))
            .MapResult<Prompt>();

        return result;
    }
}
