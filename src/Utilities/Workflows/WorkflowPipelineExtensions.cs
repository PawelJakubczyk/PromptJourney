using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Utilities.Workflows;

public static class WorkflowPipelineExtensions
{
    // --- Validate ---
    public static WorkflowPipeline Validate(this WorkflowPipeline pipeline, Func<WorkflowPipeline, WorkflowPipeline> validationBlock)
    {
        if (pipeline.BreakOnError && pipeline.Errors.Count != 0)
            return pipeline;

        var tempPipeline = WorkflowPipeline.Create([.. pipeline.Errors], breakOnError: false);

        tempPipeline = validationBlock(tempPipeline);

        pipeline.Errors.AddRange(tempPipeline.Errors);

        return pipeline;
    }

    public static async Task<WorkflowPipeline> Validate(this Task<WorkflowPipeline> pipelineTask, Func<Task<WorkflowPipeline>, Task<WorkflowPipeline>> validationBlock)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);

        if (pipeline.BreakOnError && pipeline.Errors.Count != 0)
            return pipeline;

        var tempPipelineTask = Task.FromResult(WorkflowPipeline.Create([.. pipeline.Errors], breakOnError: false));

        var validatedPipeline = await validationBlock(tempPipelineTask).ConfigureAwait(false);

        pipeline.Errors.AddRange(validatedPipeline.Errors);
        return pipeline;
    }

    // --- CollectErrors ---
    public static WorkflowPipeline CollectErrors<TValue>
    (
        this WorkflowPipeline pipeline,
        Result<TValue?>? result
    )
    {
        if (pipeline.BreakOnError && pipeline.Errors.Count != 0)
            return pipeline;

        var errorsCopy = new List<Error>(pipeline.Errors);

        if (pipeline.BreakOnError && errorsCopy.Count != 0)
            return pipeline;

        if (result is not null && result.IsFailed)
            errorsCopy.AddRange(result.Errors.OfType<Error>());

        return WorkflowPipeline.Create(errorsCopy, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> CollectErrors<TValue>
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<TValue?>? result
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.CollectErrors(result);
    }

    public static WorkflowPipeline CollectErrors<TValue>
    (
        this WorkflowPipeline pipeline,
        List<Result<TValue?>?>? results
    )
    {
        if (pipeline.BreakOnError && pipeline.Errors.Count != 0)
            return pipeline;

        var errorsCopy = new List<Error>(pipeline.Errors); ;

        if (results != null && results.Count > 0)
        {
            foreach (var result in results)
            {
                if (result.IsFailed)
                    errorsCopy.AddRange(result.Errors.OfType<Error>());
            }
        }

        return WorkflowPipeline.Create(errorsCopy, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> CollectErrors<TValue>
    (
        this Task<WorkflowPipeline> pipelineTask,
        List<Result<TValue>> results
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);

        if (pipeline.BreakOnError && pipeline.Errors.Count != 0)
            return pipeline;

        var errors = pipeline.Errors;

        foreach (var result in results)
        {
            if (result.IsFailed)
                errors.AddRange(result.Errors.OfType<Error>());
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    // --- ExecuteIfNoErrors ---
    public static WorkflowPipeline ExecuteIfNoErrors<TType>
    (
        this WorkflowPipeline pipeline,
        Func<Result<TType>> action
    )
    {
        if (pipeline.Errors.Count != 0 && pipeline.BreakOnError)
            return pipeline;

        var result = action();

        if (result.IsFailed)
        {
            pipeline.Errors.AddRange(result.Errors.OfType<Error>());
            return pipeline;
        }

        pipeline.SetResult(result.Value);

        return pipeline;
    }

    public static async Task<WorkflowPipeline> ExecuteIfNoErrors<TType>
    (
        this Task<WorkflowPipeline> pipelineTask,
        Func<Task<Result<TType>>> action
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);

        if (pipeline.BreakOnError && pipeline.Errors.Count != 0)
            return pipeline;

        var actionResult = await action().ConfigureAwait(false);

        if (actionResult.IsFailed)
        {
            pipeline.Errors.AddRange(actionResult.Errors.OfType<Error>());
            return pipeline;
        }

        pipeline.SetResult(actionResult.Value);
        return pipeline;
    }

    // --- MapResult ---
    public static Result<TResponse> MapResult<TResponse>(this WorkflowPipeline pipeline)
    {
        if (pipeline.Errors.Count != 0)
            return Result.Fail<TResponse>(pipeline.Errors);

        return Result.Ok(pipeline.GetResult<TResponse>()!);
    }

    public static Result<TResponse> MapResult<TResponse>(
        this WorkflowPipeline pipeline,
        Func<TResponse> mapResponse)
    {
        if (pipeline.Errors.Count != 0)
            return Result.Fail<TResponse>(pipeline.Errors);

        return Result.Ok(mapResponse());
    }

    public static Result<TResponse> MapResult<TSource, TResponse>(
        this WorkflowPipeline pipeline,
        Func<TSource, TResponse> mapResponse)
    {
        if (pipeline.Errors.Count != 0)
            return Result.Fail<TResponse>(pipeline.Errors);

        var source = pipeline.GetResult<TSource>();
        return Result.Ok(mapResponse(source!));
    }

    public static async Task<Result<TResponse>> MapResult<TResponse>(this Task<WorkflowPipeline> pipelineTask)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.MapResult<TResponse>();
    }

    public static async Task<Result<TResponse>> MapResult<TResponse>(
        this Task<WorkflowPipeline> pipelineTask,
        Func<TResponse> mapResponse)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.MapResult(mapResponse);
    }

    public static async Task<Result<TResponse>> MapResult<TSource, TResponse>(
        this Task<WorkflowPipeline> pipelineTask,
        Func<TSource, TResponse> mapResponse)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.MapResult(mapResponse);
    }
}