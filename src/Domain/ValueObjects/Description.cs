using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Description : ValueObject<string>, ICreatable<Description, string?>
{
    public const int MaxLength = 500;
    public static readonly Description None = new(string.Empty);
    public override bool IsNone => this == None;

    private Description(string value) :
        base(value)
    { }

    public static Result<Description> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result.Ok(None);

        value = value.Trim();

        var result = WorkflowPipeline
            .Empty()
            .CongregateErrors(
                pipeline => pipeline.IfLengthTooLong<Description>(value, MaxLength),
                pipeline => pipeline.IfContainsSuspiciousContent<Description>(value))
            .ExecuteIfNoErrors<Description>(() => new Description(value))
            .MapResult<Description>();

        return result;
    }

    public static Result<Description> Update(Result<Description> newValue)
    {

        var result = WorkflowPipeline
            .Empty()
            .CollectErrors(newValue)
            .ExecuteIfNoErrors<Description>(() => newValue)
            .MapResult<Description>();
        
        return result;
    }
}