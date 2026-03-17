using Domain.Abstractions;
using Domain.Extensions;
using System.Text.RegularExpressions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Param : ValueObject<string>, ICreatable<Param, string>
{
    public const int MaxLength = 12;
    public override bool IsNone => false;
    private Param(string value) : base(value) { }

    public static Result<Param> Create(string value)
    {
        value = value?.Trim().ToLower();

        var result = WorkflowPipeline
            .Empty()
            .IfNullOrWhitespace<Param>(value)
            .CongregateErrors(
                 pipeline => pipeline.IfLengthTooLong<Param>(value, MaxLength),
                 pipeline => pipeline.IfNotStartsWithDoubleDash(value))
            .ExecuteIfNoErrors<Param>(() => new Param(value))
            .MapResult<Param>();

        return result;
    }
}

internal static partial class ParamErrorsExtensions
{
    internal const string InvalidParamFormatMessage =
        $"Invalid Param format. Param must start with '--'.";

    internal static WorkflowPipeline IfNotStartsWithDoubleDash
    (
        this WorkflowPipeline pipeline,
        string? value
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (value is null)
            return pipeline;

        var isValid = ValidDoubleDash().IsMatch(value);

        if (!isValid)
        {
            pipeline.Errors.Add
            (
                ErrorFactories.InvalidPattern<Param>(value, InvalidParamFormatMessage)
            );
        }

        return pipeline;
    }

    [GeneratedRegex(@"--\S+", RegexOptions.Compiled)]
    private static partial Regex ValidDoubleDash();
}