using Domain.Abstractions;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Workflows;

namespace Domain.Extensions;

public static class WorkflowPipelineExtensions
{
    public static WorkflowPipeline IfNullOrWhitespace<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        string? value)
        where TLayer : ILayer
        where TValue : ValueObject<string?>
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (string.IsNullOrWhiteSpace(value))
        {
            pipeline.Errors.Add(
                ErrorFactories.NullOrWhitespace<TValue, TLayer>()
            );
        }

        return pipeline;
    }

    public static WorkflowPipeline IfWhitespace<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        string? value)
        where TLayer : ILayer
        where TValue : ValueObject<string?>?
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (value != null && string.IsNullOrWhiteSpace(value))
        {
            pipeline.Errors.Add(
                ErrorFactories.Whitespace<TValue, TLayer>()
            );
        }

        return pipeline;
    }

    public static WorkflowPipeline IfLengthTooLong<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        string? value,
        int maxLength)
        where TLayer : ILayer
        where TValue : ValueObject<string?>
    {
        if (pipeline.BreakOnError)
            return pipeline;

        if (value?.Length > maxLength)
        {
            pipeline.Errors.Add(
                ErrorFactories.TooLong<TValue, TLayer>(value, maxLength)
            );
        }

        return pipeline;
    }

    public static WorkflowPipeline IfListHasDuplicates<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        List<TValue>? values)
        where TLayer : ILayer
        where TValue : ValueObject<string?>?
    {
        if (pipeline.BreakOnError)
            return pipeline;

        var duplicates = values?
            .GroupBy(v => v)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates is { Count: > 0 })
        {
            pipeline.Errors.Add(
                ErrorFactories.DuplicateItems<TValue, TLayer>(duplicates)
            );
        }

        return pipeline;
    }

}

