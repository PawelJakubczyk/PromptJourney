//using FluentResults;
//using Utilities.Validation;

//namespace Application.Extension;

//public static class RepositoryActionExtensions
//{
//    public static async Task<Result<TResponse>> ExecuteAndMapResultIfNoErrors<TType, TResponse>(
//    this Task<ValidationPipeline> pipelineTask,
//    Func<Task<Result<TType>>> repositoryAction,
//    Func<TType, TResponse> mapResponse)
//    {
//        var pipeline = await pipelineTask;
//        var errors = pipeline.Errors;

//        if (errors.Count != 0)
//            return Result.Fail<TResponse>(errors);

//        var result = await repositoryAction();

//        if (result.IsFailed)
//        {
//            errors.AddRange(result.Errors.OfType<Error>());
//            return Result.Fail<TResponse>(errors);
//        }

//        var response = mapResponse(result.Value);
//        return Result.Ok(response);
//    }

//    public static async Task<ValidationPipeline> CollectErrors<TValue>(
//    this Task<ValidationPipeline> pipelineTask,
//    Result<TValue>? result)
//    {
//        var pipeline = await pipelineTask.ConfigureAwait(false);
//        var errors = pipeline.Errors;

//        if (pipeline.BreakOnError && errors.Count != 0)
//            return pipeline;

//        if (result is not null && result.IsFailed)
//            errors.AddRange(result.Errors.OfType<Error>());

//        return ValidationPipeline.Create(errors, pipeline.BreakOnError);
//    }

//    public static async Task<ValidationPipeline> BeginValidationBlock(this Task<ValidationPipeline> pipelineTask)
//    {
//        var pipeline = await pipelineTask;
//        var errorsTask = pipeline.Errors;

//        return ValidationPipeline.Create(errorsTask, breakOnError: false);
//    }

//    public static async Task<ValidationPipeline> EndValidationBlock(this Task<ValidationPipeline> pipelineTask)
//    {
//        var pipeline = await pipelineTask;
//        var errorsTask = pipeline.Errors;

//        return ValidationPipeline.Create(errorsTask, breakOnError: true);
//    }
//}

