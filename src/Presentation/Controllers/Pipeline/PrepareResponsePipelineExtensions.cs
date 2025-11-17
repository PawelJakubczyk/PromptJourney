using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Pipeline;

internal static class PrepareResponsePipelineExtensions
{
    public static async Task<Pipeline<TResponse>> ElsePrepareOKResponse<TResponse>
    (
        this Task<Pipeline<TResponse>> pipelineTask,
        Func<TResponse?,
        IActionResult>? bodyFactory = null)
    {
        return await ElsePrepareResponse(pipelineTask, pipeline => pipeline.PrepareOKResponse(bodyFactory))
            .ConfigureAwait(false);
    }

    public static async Task<Pipeline<TResponse>> ElsePrepareCreateResponse<TResponse>
    (
        this Task<Pipeline<TResponse>> pipelineTask,
        Func<TResponse?, IActionResult>? bodyFactory = null)
    {
        return await ElsePrepareResponse(pipelineTask, pipeline => pipeline.PrepareCreateResponse(bodyFactory))
            .ConfigureAwait(false);
    }

    private static async Task<Pipeline<TResponse>> ElsePrepareResponse<TResponse>(
        Task<Pipeline<TResponse>> pipelineTask,
        Func<Pipeline<TResponse>, Pipeline<TResponse>> prepareFunc)
    {
        ArgumentNullException.ThrowIfNull(pipelineTask);

        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.Result.IsFailed ? pipeline : prepareFunc(pipeline);
    }
}
