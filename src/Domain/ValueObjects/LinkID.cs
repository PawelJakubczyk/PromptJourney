using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record LinkID : ValueObject<Guid>, ICreatable<LinkID, string?>
{
    public const int ExactLength = 32;
    public override bool IsNone => false;

    private LinkID(Guid value) : base(value) { }

    public static Result<LinkID> Create(string? value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfLinkIdNullOrWhitespace(value)
            .CongregateErrors(
                pipeline => pipeline.IfLengthNotExact<LinkID>(value!, ExactLength),
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

file static partial class LinkIDErrorsExtensions
{

    internal static WorkflowPipeline IfLinkIdNullOrWhitespace
    (
        this WorkflowPipeline pipeline,
        string? value
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (string.IsNullOrWhiteSpace(value))
        {
            pipeline.Errors.Add(
                ErrorFactories.NullOrWhitespace<LinkID>(value)
            );
        }

        return pipeline;
    }
}