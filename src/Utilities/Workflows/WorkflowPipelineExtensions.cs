using Utilities.Errors;
using Utilities.Results;

namespace Utilities.Workflows;

public static class WorkflowPipelineExtensions
{
    // --- Congregate ---
    public static WorkflowPipeline Congregate
    (
        this WorkflowPipeline pipeline, 
        params Func<WorkflowPipeline, WorkflowPipeline>[] validationBlocks
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        var tempPipeline = WorkflowPipeline.Create([.. pipeline.Errors], breakOnError: false);

        foreach (var validationBlock in validationBlocks)
        {
            var beforeCount = tempPipeline.Errors.Count;
            tempPipeline = validationBlock(tempPipeline);
            var newErrors = tempPipeline.Errors.Skip(beforeCount);
            pipeline.Errors.AddRange(newErrors);
        }

        return pipeline;
    }

    public static async Task<WorkflowPipeline> Congregate
    (
        this Task<WorkflowPipeline> pipelineTask,
        params Func<Task<WorkflowPipeline>, Task<WorkflowPipeline>>[] validationBlocks)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);

        if (pipeline.BreakOnError)
            return pipeline;

        var tempPipeline = WorkflowPipeline.Create([.. pipeline.Errors], breakOnError: false);

        foreach (var validationBlock in validationBlocks)
        {
            var beforeCount = tempPipeline.Errors.Count;
            tempPipeline = await validationBlock(Task.FromResult(tempPipeline)).ConfigureAwait(false);
            var newErrors = tempPipeline.Errors.Skip(beforeCount);
            pipeline.Errors.AddRange(newErrors);
        }

        return pipeline;
    }

    // --- CollectErrors ---
    public static WorkflowPipeline CollectErrors<TValue>
    (
        this WorkflowPipeline pipeline,
        params Result<TValue>[] results
    )
    {
        if (pipeline.BreakOnError)
            return pipeline;

        var errorsCopy = new List<Error>(pipeline.Errors);

        foreach (var result in results)
        {
            if (result is not null && result.IsFailed)
                errorsCopy.AddRange(result.Errors.OfType<Error>());
        }

        return WorkflowPipeline.Create(errorsCopy, pipeline.BreakOnError);
    }

    public static async Task<WorkflowPipeline> CollectErrors<TValue>
    (
        this Task<WorkflowPipeline> pipelineTask,
        params Result<TValue>[] results
    )   
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);

        return pipeline.CollectErrors(results);
    }

    // --- ExecuteIfNoErrors ---
    public static WorkflowPipeline ExecuteIfNoErrors<TType>
    (
        this WorkflowPipeline pipeline,
        Func<Result<TType>> action
    )
    {
        if (pipeline.BreakOnError || pipeline.Errors.Count > 0)
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

        if (pipeline.BreakOnError || pipeline.Errors.Count > 0)
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

    public static Result<TResponse> MapResult<TResponse>
    (
        this WorkflowPipeline pipeline,
        Func<TResponse> mapResponse
    )
    {
        if (pipeline.Errors.Count != 0)
            return Result.Fail<TResponse>(pipeline.Errors);

        return Result.Ok(mapResponse());
    }

    public static Result<TResponse> MapResult<TSource, TResponse>
    (
        this WorkflowPipeline pipeline,
        Func<TSource, TResponse> mapResponse
    )
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

    public static async Task<Result<TResponse>> MapResult<TResponse>
    (
        this Task<WorkflowPipeline> pipelineTask,
        Func<TResponse> mapResponse
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.MapResult(mapResponse);
    }

    public static async Task<Result<TResponse>> MapResult<TSource, TResponse>
    (
        this Task<WorkflowPipeline> pipelineTask,
        Func<TSource, TResponse> mapResponse
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.MapResult(mapResponse);
    }
}
