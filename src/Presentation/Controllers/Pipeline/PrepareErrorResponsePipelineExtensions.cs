using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Errors;

namespace Presentation.Controllers.Pipeline;

internal static class PrepareErrorResponsePipelineExtensions
{
    public static async Task<Pipeline<TResponse>> IfErrorsPrepareErrorResponse<TResponse>
(
    this Task<Result<TResponse>> sourceTask,
    Func<Error, IEnumerable<object>,
    IActionResult>? bodyFactory = null
)
    {
        ArgumentNullException.ThrowIfNull(sourceTask);

        var result = await sourceTask;
        var pipeline = new Pipeline<TResponse>(result);

        if (!pipeline.Result.IsFailed)
        {
            return pipeline;
        }

        var errors = result.Errors.OfType<Error>().ToList();

        var mainError = errors.Count != 0
            ? Pipeline<TResponse>.PickHighestPriorityErrorInternal(errors)
            : ErrorFactories.Unknown<PresentationLayer>();

        var details = errors.Select(error => error.GetDetail()).ToList();

        pipeline.SetResponse(bodyFactory != null
            ? bodyFactory(mainError, details)
            : new ObjectResult(new
            {
                mainError = new { code = mainError.GetErrorCode(), message = mainError.Message },
                details
            })
            {
                StatusCode = mainError.GetErrorCode()
            });

        return pipeline;
    }
}
