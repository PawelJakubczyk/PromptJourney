using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record LinkID : ValueObject<Guid>, ICreatable<LinkID, string?>
{
    public const int ExactLength = 36;
    public override bool IsNone => false;

    private LinkID(Guid value) : base(value) { }

    public static Result<LinkID> Create(string? value)
    {
        value = value?.Trim();

        if (string.IsNullOrEmpty(value))
        {
            Create();
        }

        var result = WorkflowPipeline
            .Empty()
            .AggregateErrors(
                pipeline => pipeline.IfLengthTooLong<LinkID, Guid>(value!, ExactLength),
                pipeline => pipeline.IfGuidFormatInvalid(value!))
            .ExecuteIfNoErrors<LinkID>(() => new LinkID(Guid.Parse(value!)))
            .MapResult<LinkID>();

        return result;
    }

    public static Result<LinkID> Create()
    {
        var value = Guid.NewGuid();

        var result = Result.Ok(new LinkID(value));
        return result;
    }
}