using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record UserID : ValueObject<Guid>, ICreatable<UserID, string?>
{
    public const int ExactLength = 36;
    public override bool IsNone => false;

    private UserID(Guid value) : base(value) { }

    public static Result<UserID> Create(string? value)
    {
        value = value?.Trim();

        if (string.IsNullOrEmpty(value))
            return Create();

        var result = WorkflowPipeline
            .Empty()
            .AggregateErrors(
                pipeline => pipeline.IfLengthTooLong<UserID, Guid>(value!, ExactLength),
                pipeline => pipeline.IfGuidFormatInvalid(value!))
            .ExecuteIfNoErrors<UserID>(() => new UserID(Guid.Parse(value!)))
            .MapResult<UserID>();

        return result;
    }

    public static Result<UserID> Create()
    {
        var value = Guid.NewGuid();
        return Result.Ok(new UserID(value));
    }
}
