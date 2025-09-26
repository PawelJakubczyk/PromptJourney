using FluentResults;

namespace Utilities.Validation;

public static class WorkflowPipelineExtensions
{
    // --- Validate ---
    public static WorkflowPipeline Validate(this WorkflowPipeline pipeline, Func<WorkflowPipeline, WorkflowPipeline> validationBlock)
    {
        var tempPipeline = WorkflowPipeline.Create(pipeline.Errors, breakOnError: false);

        tempPipeline = validationBlock(tempPipeline);

        pipeline.Errors.AddRange(tempPipeline.Errors);

        return pipeline;
    }

    public static async Task<WorkflowPipeline> Validate(this Task<WorkflowPipeline> pipelineTask, Func<Task<WorkflowPipeline>, Task<WorkflowPipeline>> validationBlock)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        var tempPipelineTask = Task.FromResult(WorkflowPipeline.Create(pipeline.Errors, breakOnError: false));

        var validatedPipeline = await validationBlock(tempPipelineTask).ConfigureAwait(false);

        pipeline.Errors.AddRange(validatedPipeline.Errors);
        return pipeline;
    }

    // --- CollectErrors ---
    public static WorkflowPipeline CollectErrors<TValue>
    (
        this WorkflowPipeline pipeline,
        Result<TValue>? result
    )
    {
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (result is not null && result.IsFailed)
            errors.AddRange(result.Errors.OfType<Error>());

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> CollectErrors<TValue>
    (
        this Task<WorkflowPipeline> pipelineTask,
        Result<TValue>? result
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.CollectErrors(result);
    }

    public static WorkflowPipeline CollectErrors<TValue>
    (
        this WorkflowPipeline pipeline,
        List<Result<TValue>> results
    )
    {
        var errors = pipeline.Errors;

        foreach (var result in results)
        {
            if (result.IsFailed)
                errors.AddRange(result.Errors.OfType<Error>());
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> CollectErrors<TValue>
    (
        this Task<WorkflowPipeline> pipelineTask,
        List<Result<TValue>> results
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);

        var errors = pipeline.Errors;

        foreach (var result in results)
        {
            if (result.IsFailed)
                errors.AddRange(result.Errors.OfType<Error>());
        }

        return WorkflowPipeline.Create(errors, pipeline.BreakOnError);
    }
}

public static class RepositoryActionExtensions
{
    public static Result<T> ExecuteIfNoErrors<T>
    (
        this WorkflowPipeline pipeline,
        Func<Result<T>> action)
    {
        if (pipeline.Errors.Count != 0)
            return Result.Fail<T>(pipeline.Errors);

        return action();
    }

    public static async Task<Result<T>> ExecuteIfNoErrors<T>
    (
        this Task<WorkflowPipeline> pipelineTask,
        Func<Task<Result<T>>> action)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.ExecuteIfNoErrors(() => action().GetAwaiter().GetResult());
    }

    public static Result<TResponse> MapResult<TSource, TResponse>(this Result<TSource> result, Func<TSource, TResponse> mapResponse)
    {
        if (result.IsFailed)
            return Result.Fail<TResponse>(result.Errors);

        return Result.Ok(mapResponse(result.Value));
    }

    public static async Task<Result<TResponse>> MapResult<TSource, TResponse>(this Task<Result<TSource>> resultTask, Func<TSource, TResponse> mapResponse)
    {
        var result = await resultTask.ConfigureAwait(false);
        return result.MapResult(mapResponse);
    }
}
