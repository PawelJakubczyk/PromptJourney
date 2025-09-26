using Domain.Abstractions;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Validation;

namespace Domain.Extensions;

public static class WorkflowPipelineExtensions
{
    public static WorkflowPipeline IfNullOrWhitespace<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        string? value)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            pipeline.Errors.Add(
                new Error<TLayer>($"{typeof(TValue).Name}: value cannot be null or whitespace.", StatusCodes.Status400BadRequest)
            );
        }
        return pipeline;
    }

    public static WorkflowPipeline IfWhitespace<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        string? value)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (!string.IsNullOrEmpty(value) && string.IsNullOrWhiteSpace(value))
        {
            pipeline.Errors.Add(
                new Error<TLayer>($"{typeof(TValue).Name}: cannot be whitespace.", StatusCodes.Status400BadRequest)
            );
        }
        return pipeline;
    }

    public static WorkflowPipeline IfLengthTooLong<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        string? value,
        int maxLength)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (value?.Length > maxLength)
        {
            pipeline.Errors.Add(
                new Error<TLayer>($"{typeof(TValue).Name}: '{value}' cannot be longer than {maxLength} characters.", StatusCodes.Status400BadRequest)
            );
        }
        return pipeline;
    }

    public static WorkflowPipeline IfListHasDuplicates<TLayer, TValue>(
        this WorkflowPipeline pipeline,
        List<TValue>? values)
        where TLayer : ILayer
        where TValue : ValueObject<string>
    {
        if (values is null)
            return pipeline;

        var duplicates = values
            .GroupBy(v => v)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicates.Count != 0)
        {
            var duplicateNames = string.Join(", ", duplicates.Select(d => d.ToString()));
            pipeline.Errors.Add(
                new Error<TLayer>($"{typeof(TValue).Name}: contains duplicates -> {duplicateNames}.", StatusCodes.Status400BadRequest)
            );
        }

        return pipeline;
    }
}