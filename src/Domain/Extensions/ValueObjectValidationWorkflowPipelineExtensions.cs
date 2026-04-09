using Domain.Abstractions;
using Domain.ValueObjects;
using System.Globalization;
using System.Text.RegularExpressions;
using Utilities.Errors;
using Utilities.Results;
using Utilities.Workflows;

namespace Domain.Extensions;

public static partial class ValueObjectValidationWorkflowPipelineExtensions
{

    internal static WorkflowPipeline IfNullOrWhitespace<TValue>
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
                ErrorFactories.NullOrWhitespace<TValue>(value)
            );
        }

        return pipeline;
    }

    public static WorkflowPipeline IfGuidFormatInvalid
    (
        this WorkflowPipeline pipeline,
        string? value
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (value is null)
            return pipeline;

        if (!Guid.TryParse(value, out _))
        {
            pipeline.Errors.Add
            (
                ErrorFactories.InvalidPattern<HistoryID>(value, "Invalid GUID format.")
            );
        }

        return pipeline;
    }

    public static WorkflowPipeline IfLengthNotExact<TValue>
    (
        this WorkflowPipeline pipeline,
        string value,
        int exactLength
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (value.Length != exactLength)
        {
            pipeline.Errors.Add
            (
                ErrorFactories.InvalidLength<TValue>(value, exactLength)
            );
        }
        return pipeline;
    }

    public static WorkflowPipeline IfLengthTooLong<TValue>
    (
        this WorkflowPipeline pipeline,
        string value,
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

    internal static WorkflowPipeline IfDateFormatInvalid<TValue>
(
    this WorkflowPipeline pipeline,
    string input
)
    {
        if (pipeline.BreakOnError)
            return pipeline;

        var isValid = DateTimeOffset.TryParse(
            input,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AdjustToUniversal,
            out _
        );

        if (!isValid)
        {
            pipeline.Errors.Add(ErrorFactories.InvalidDateFormat<TValue>(input));
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

