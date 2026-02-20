
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Errors;
using Utilities.Results;

namespace Presentation.Controllers.Pipeline;

internal static class PrepareErrorResponsePipelineExtensions
{
    public static async Task<Pipeline<TResponse>> IfErrorsPrepareErrorResponse<TResponse>(
        this Task<Result<TResponse>> sourceTask,
        Func<Error, IEnumerable<string>, IActionResult>? bodyFactory = null)
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

        var errorMessages = errors.Select(error => error.Message).ToList();

        pipeline.SetResponse(bodyFactory != null
            ? bodyFactory(mainError, errorMessages)
            : new ObjectResult(new
            {
                mainError = new
                {
                    code = mainError.GetErrorCode(),
                    message = mainError.Message,
                    layer = mainError.GetLayer()
                },
                errors = errorMessages
            })
            {
                StatusCode = mainError.GetErrorCode()
            });

        return pipeline;
    }
}
