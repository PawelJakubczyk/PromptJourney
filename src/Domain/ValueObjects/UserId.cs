using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record UserId : ValueObject<Guid>, ICreatable<UserId, string?>
{
    public const int ExactLength = 36;
    public override bool IsNone => false;

    private UserId(Guid value) : base(value) { }

    public static Result<UserId> Create(string? value)
    {
        value = value?.Trim();

        if (string.IsNullOrEmpty(value))
            return Create();

        var result = WorkflowPipeline
            .Empty()
            .AggregateErrors(
                pipeline => pipeline.IfLengthTooLong<UserId, Guid>(value!, ExactLength),
                pipeline => pipeline.IfGuidFormatInvalid(value!))
            .ExecuteIfNoErrors<UserId>(() => new UserId(Guid.Parse(value!)))
            .MapResult<UserId>();

        return result;
    }

    public static Result<UserId> Create()
    {
        var value = Guid.NewGuid();
        return Result.Ok(new UserId(value));
    }
}
