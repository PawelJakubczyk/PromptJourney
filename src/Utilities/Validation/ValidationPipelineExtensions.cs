using FluentResults;
using System.Threading.Tasks;

namespace Utilities.Validation;

public static class ValidationPipelineExtensions
{
    // --- BeginValidationBlock ---
    public static ValidationPipeline BeginValidationBlock(this ValidationPipeline pipeline)
    {
        return ValidationPipeline.Create(pipeline.Errors, breakOnError: false);
    }

    public static async Task<ValidationPipeline> BeginValidationBlock(this Task<ValidationPipeline> pipelineTask)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.BeginValidationBlock();
    }

    // --- EndValidationBlock ---
    public static ValidationPipeline EndValidationBlock(this ValidationPipeline pipeline)
    {
        return ValidationPipeline.Create(pipeline.Errors, breakOnError: true);
    }

    public static async Task<ValidationPipeline> EndValidationBlock(this Task<ValidationPipeline> pipelineTask)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.EndValidationBlock();
    }

    // --- CollectErrors ---
    public static ValidationPipeline CollectErrors<TValue>
    (
        this ValidationPipeline pipeline,
        Result<TValue>? result
    )
    {
        var errors = pipeline.Errors;

        if (pipeline.BreakOnError && errors.Count != 0)
            return pipeline;

        if (result is not null && result.IsFailed)
            errors.AddRange(result.Errors.OfType<Error>());

        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
    }

    public static async Task<ValidationPipeline> CollectErrors<TValue>
    (
        this Task<ValidationPipeline> pipelineTask,
        Result<TValue>? result
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.CollectErrors(result);
    }
}

public static class RepositoryActionExtensions
{
    // --- ExecuteAndMapResultIfNoErrors ---
    public static Result<TResponse> ExecuteAndMapResultIfNoErrors<TType, TResponse>
    (
        this ValidationPipeline pipeline,
        Func<Result<TType>> repositoryAction,
        Func<TType, TResponse> mapResponse
    )
    {
        var errors = pipeline.Errors;

        if (errors.Count != 0)
            return Result.Fail<TResponse>(errors);

        var result = repositoryAction();

        if (result.IsFailed)
        {
            errors.AddRange(result.Errors.OfType<Error>());
            return Result.Fail<TResponse>(errors);
        }

        var response = mapResponse(result.Value);
        return Result.Ok(response);
    }

    public static async Task<Result<TResponse>> ExecuteAndMapResultIfNoErrors<TType, TResponse>
    (
        this Task<ValidationPipeline> pipelineTask,
        Func<Task<Result<TType>>> repositoryAction,
        Func<TType, TResponse> mapResponse
    )
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.ExecuteAndMapResultIfNoErrors(
            () => repositoryAction().GetAwaiter().GetResult(),
            mapResponse
        );
    }

    public static Result IfNoErrors(this ValidationPipeline pipeline)
    {
        if (pipeline.Errors.Count != 0)
            return Result.Fail(pipeline.Errors);

        return Result.Ok();
    }

    public static async Task<Result> IfNoErrors(this Task<ValidationPipeline> pipelineTask)
    {
        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.IfNoErrors();
    }

    public static Result<T> Executes<T>(this Result validationResult, Func<Result<T>> action)
    {
        if (validationResult.IsFailed)
            return Result.Fail<T>(validationResult.Errors);

        return action();
    }

    public static async Task<Result<T>> Executes<T>(this Task<Result> validationResultTask, Func<Task<Result<T>>> action)
    {
        var validationResult = await validationResultTask.ConfigureAwait(false);
        if (validationResult.IsFailed)
            return Result.Fail<T>(validationResult.Errors);

        return await action().ConfigureAwait(false);
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
