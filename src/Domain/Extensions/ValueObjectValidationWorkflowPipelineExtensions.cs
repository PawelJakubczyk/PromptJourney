using Domain.Abstractions;
using Domain.ValueObjects;
using System.Text.RegularExpressions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Extensions;

public static partial class ValueObjectValidationWorkflowPipelineExtensions
{
    public static WorkflowPipeline IfNullOrWhitespace<TValue>
    (
        this WorkflowPipeline pipeline,
        string? value)
        where TValue : ValueObject<string>
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (string.IsNullOrWhiteSpace(value))
        {
            pipeline.Errors.Add(
                ErrorFactories.NullOrWhitespace<TValue>(value)
            );
        }

        return pipeline;
    }

    internal static WorkflowPipeline IfReleaseDateNullOrWhitespace
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
                ErrorFactories.NullOrWhitespace<ReleaseDate>(value)
            );
        }

        return pipeline;
    }

    public static WorkflowPipeline IfLengthTooLong<TValue>
    (
        this WorkflowPipeline pipeline,
        string? value,
        int maxLength)
        where TValue : ValueObject<string>
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (value?.Length > maxLength)
        {
            pipeline.Errors.Add(
                ErrorFactories.TooLong<TValue>(value, maxLength)
            );
        }

        return pipeline;
    }

    public static WorkflowPipeline IfContainsSuspiciousContent<TValue>
    (
        this WorkflowPipeline pipeline,
        string value
    )
        where TValue : ValueObject<string>
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (XssPatternRegex().IsMatch(value ?? string.Empty))
        {
            pipeline.Errors.Add(
                ErrorFactories.SuspiciousContent<TValue>(value!)
            );
        }

        return pipeline;
    }

    public static WorkflowPipeline CollectErrorsFromList<TValue>
    (
        this WorkflowPipeline pipeline,
        List<Result<TValue>> results
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        var errorsCopy = new List<Error>(pipeline.Errors);

        foreach (var result in results ?? [])
        {
            if (result is not null && result.IsFailed)
                errorsCopy.AddRange(result.Errors.OfType<Error>());
        }

        return WorkflowPipeline.Create(errorsCopy, pipeline.BreakOnError);
    }

    [GeneratedRegex(@"<script|javascript:|on\w+\s*=|<iframe|<object|<embed|<img\s+src", RegexOptions.IgnoreCase)]
    private static partial Regex XssPatternRegex();
}

