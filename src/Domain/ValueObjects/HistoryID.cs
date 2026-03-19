using Domain.Abstractions;
using Domain.Extensions;
using Utilities.Errors;
using Utilities.Workflows;
using Utilities.Results;

namespace Domain.ValueObjects;

public record HistoryID : ValueObject<Guid>, ICreatable<HistoryID, string?>
{
    public const int ExactLength = 32;
    public override bool IsNone => false;
    public Guid ValueAsGuid { get; }

    private HistoryID(Guid value) : base(value) 
    {
        ValueAsGuid = value;
    }

    public static Result<HistoryID> Create(string? value)
    {
        value = value?.Trim();

        var result = WorkflowPipeline
            .Empty()
            .IfHistoryIdNullOrWhitespace(value)
            .CongregateErrors(
                pipeline => pipeline.IfLengthNotExact<HistoryID>(value!, ExactLength),
                pipeline => pipeline.IfGuidFormatInvalid(value!))
            .ExecuteIfNoErrors<HistoryID>(() => new HistoryID(Guid.Parse(value!)))
            .MapResult<HistoryID>();

        return result;
    }

    public static Result<HistoryID> Create()
    {
        var value = Guid.NewGuid();

        var result = Result.Ok(new HistoryID(value));

        return result;
    }
}

file static partial class HistoryIDErrorsExtensions
{

    internal static WorkflowPipeline IfHistoryIdNullOrWhitespace
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
                ErrorFactories.NullOrWhitespace<HistoryID>(value)
            );
        }

        return pipeline;
    }
}