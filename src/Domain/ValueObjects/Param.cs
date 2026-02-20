using Domain.Abstractions;
using Domain.Extensions;
using System.Text.RegularExpressions;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.ValueObjects;

public record Param : ValueObject<string>, ICreatable<Param, string?>
{
    public const int MaxLength = 12;

    private Param(string value) : base(value) { }

    public static Result<Param> Create(string? value)
    {
        var result = WorkflowPipeline
            .Empty()
            .Congregate(
                 pipeline => pipeline.IfNullOrWhitespace<DomainLayer, Param>(value),
                 pipeline => pipeline.IfNotStartsWithDoubleDash<DomainLayer>(value),
                 pipeline => pipeline.IfLengthTooLong<DomainLayer, Param>(value, MaxLength))
            .ExecuteIfNoErrors<Param>(() => new Param(value))
            .MapResult<Param>();

        return result;
    }
}

internal static partial class ParamErrorsExtensions
{
    internal const string InvalidParamFormatMessage =
        $"Invalid Param format. Param must start with '--'.";

    internal static WorkflowPipeline IfNotStartsWithDoubleDash<TLayer>
    (
        this WorkflowPipeline pipeline,
        string? value
    )
        where TLayer : ILayer
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
                ErrorFactories.InvalidPattern<Param, TLayer>(value, InvalidParamFormatMessage)
            );
        }

        return pipeline;
    }

    [GeneratedRegex(@"--\S+", RegexOptions.Compiled)]
    private static partial Regex ValidDoubleDash();
}