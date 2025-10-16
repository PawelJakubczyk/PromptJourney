using Domain.Abstractions;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;
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
            pipeline.Errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<TLayer>()
                .WithMessage($"{typeof(TValue).Name}: value cannot be null or whitespace.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
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
            pipeline.Errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<TLayer>()
                .WithMessage($"{typeof(TValue).Name}: cannot be whitespace.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
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
            pipeline.Errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<TLayer>()
                .WithMessage($"{typeof(TValue).Name}: '{value}' cannot be longer than {maxLength} characters.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
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
            .GroupBy(value => value)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        if (duplicates?.Count != 0 && duplicates is not null)
        {
            var duplicateNames = string.Join(", ", duplicates.Select(d => d.ToString()));
            pipeline.Errors.Add
            (
            ErrorBuilder.New()
                .WithLayer<TLayer>()
                .WithMessage($"{typeof(TValue).Name}: contains duplicates -> {duplicateNames}.")
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build()
            );
        }

        return pipeline;
    }
}
