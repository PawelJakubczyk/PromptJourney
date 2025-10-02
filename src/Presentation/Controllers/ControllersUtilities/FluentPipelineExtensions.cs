using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Extensions;
using static Presentation.Constants.StatusPriority;

namespace Presentation.Controllers.ControllersUtilities;

public sealed class Pipeline<TResponse>
{
    internal Result<TResponse> Result { get; }
    internal IActionResult Response { get; private set; }

    internal Pipeline(Result<TResponse> result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    public Pipeline<TResponse> PrepareErrorResponse(Func<Error, IEnumerable<object>, IActionResult>? bodyFactory = null)
    {
        var Errors = Result.Errors.OfType<Error>().ToList();

        var mainError = Errors.Count != 0
            ? PickHighestPriorityErrorInternal(Errors)
            : (ErrorFactory.Create()
                .Withlayer(typeof(PresentationLayer))
                .WithMessage("Unknown error")
                .WithErrorCode(StatusCodes.Status500InternalServerError));


        var details = Errors.Select(error => error.GetDetail()).ToList();

        Response = bodyFactory != null
            ? bodyFactory(mainError, details)
            : new ObjectResult(new
            {
                mainError = new { code = mainError.GetErrorCode(), message = mainError.Message },
                details
            })
            {
                StatusCode = mainError.GetErrorCode()
            };

        return this;
    }

    public Pipeline<TResponse> PrepareOKResponse(Func<TResponse?, IActionResult>? bodyFactory = null)
    {
        TResponse? payload = Result.IsSuccess ? Result.Value : default;
        Response = bodyFactory != null
            ? bodyFactory(payload)
            : new OkObjectResult(payload);
        return this;
    }

    public static Error PickHighestPriorityErrorInternal(List<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        if (errors.Count == 0) throw new ArgumentException("errors must contain at least one item", nameof(errors));

        var mainErrorCode = errors
            .Select(er => er.GetErrorCode())
            .Where(code => code.HasValue)
            .OrderBy(code => StatusPriorityDict.GetValueOrDefault(code!.Value, int.MaxValue))
            .FirstOrDefault() ?? StatusCodes.Status500InternalServerError;

        var mainError = errors
            .Where(e => e.GetErrorCode() == mainErrorCode)
            .FirstOrDefault() ?? new Error("An error occurred");

        return mainError;
    }
}

public static class PipelineExtensions
{
    public static async Task<Pipeline<TResponse>> IfErrors<TResponse>(this Task<Result<TResponse>> sourceTask, Func<Pipeline<TResponse>, Pipeline<TResponse>> branch)
    {
        ArgumentNullException.ThrowIfNull(sourceTask);
        ArgumentNullException.ThrowIfNull(branch);

        var result = await sourceTask.ConfigureAwait(false);
        var pipeline = new Pipeline<TResponse>(result);

        return pipeline.Result.IsFailed ? branch(pipeline) : pipeline;
    }

    public static async Task<Pipeline<T>> Else<T>(this Task<Pipeline<T>> pipelineTask, Func<Pipeline<T>, Pipeline<T>> branch)
    {
        ArgumentNullException.ThrowIfNull(pipelineTask);
        ArgumentNullException.ThrowIfNull(branch);

        var pipeline = await pipelineTask.ConfigureAwait(false);
        return pipeline.Result.IsFailed ? pipeline : branch(pipeline);
    }

    public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Pipeline<T>> pipelineTask)
    {
        ArgumentNullException.ThrowIfNull(pipelineTask);

        var pipeline = await pipelineTask.ConfigureAwait(false);
        ArgumentNullException.ThrowIfNull(pipeline);

        return pipeline.Response;
    }
}