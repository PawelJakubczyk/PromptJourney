using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using Utilities.Constants;
using Utilities.Extensions;
using static Presentation.Constants.StatusPriority;

namespace Presentation.Controllers.Utilities;

public sealed class Pipeline<TResponse>
{
    internal Result<TResponse> Result { get; }
    internal IActionResult Response { get; private set; }

    internal Pipeline(Result<TResponse>? result)
    {
        Result = result ?? throw new ArgumentNullException(nameof(result));
        Response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
    }

    internal void SetResponse(IActionResult response)
    {
        Response = response ?? throw new ArgumentNullException(nameof(response));
    }

    public Pipeline<TResponse> PrepareOKResponse(Func<TResponse?, IActionResult>? bodyFactory = null)
    {
        TResponse? payload = Result.IsSuccess ? Result.Value : default;
        Response = bodyFactory != null
            ? bodyFactory(payload)
            : new OkObjectResult(payload);
        return this;
    }

    public Pipeline<TResponse> PrepareCreateResponse(Func<TResponse?, IActionResult>? bodyFactory = null)
    {
        TResponse? payload = Result.IsSuccess ? Result.Value : default;
        Response = bodyFactory != null
            ? bodyFactory(payload)
            : new CreatedResult(string.Empty, payload);
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
    public static async Task<Pipeline<TResponse>> IfErrorsPrepareErrorResponse<TResponse>(
        this Task<Result<TResponse>> sourceTask,
        Func<Error, IEnumerable<object>, IActionResult>? bodyFactory = null)
    {
        ArgumentNullException.ThrowIfNull(sourceTask);

        var result = await sourceTask.ConfigureAwait(false);
        var pipeline = new Pipeline<TResponse>(result);

        if (!pipeline.Result.IsFailed)
        {
            return pipeline;
        }

        var errors = result.Errors.OfType<Error>().ToList();

        var mainError = errors.Count != 0
            ? Pipeline<TResponse>.PickHighestPriorityErrorInternal(errors)
            : ErrorBuilder.New()
                .WithLayer<PresentationLayer>()
                .WithMessage("Unknown error")
                .WithErrorCode(StatusCodes.Status500InternalServerError)
                .Build();

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

    public static async Task<Pipeline<TResponse>> ElsePrepareOKResponse<TResponse>(
        this Task<Pipeline<TResponse>> pipelineTask,
        Func<TResponse?, IActionResult>? bodyFactory = null)
    {
        return await ElsePrepareResponse(pipelineTask, p => p.PrepareOKResponse(bodyFactory))
            .ConfigureAwait(false);
    }

    public static async Task<Pipeline<TResponse>> ElsePrepareCreateResponse<TResponse>(
        this Task<Pipeline<TResponse>> pipelineTask,
        Func<TResponse?, IActionResult>? bodyFactory = null)
    {
        return await ElsePrepareResponse(pipelineTask, p => p.PrepareCreateResponse(bodyFactory))
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

    // shared helpers
    private static (ProblemDetails details, int status) BuildProblemDetailsFromErrors(IEnumerable<IError> errors)
    {
        var list = errors.OfType<Error>().ToList();
        var mainError = list.Count != 0
            ? Pipeline<object>.PickHighestPriorityErrorInternal(list)
            : ErrorBuilder.New()
                .WithLayer<PresentationLayer>()
                .WithMessage("Unknown error")
                .WithErrorCode(StatusCodes.Status500InternalServerError)
                .Build();

        var detailMessages = list.Select(e => e.GetDetail()).ToList();
        var pdMain = new ProblemDetails
        {
            Title = mainError.Message,
            Detail = string.Join(" | ", detailMessages),
            Status = mainError.GetErrorCode()
        };

        var status = mainError.GetErrorCode() ?? StatusCodes.Status500InternalServerError;
        return (pdMain, status);
    }

    private static async Task<(bool success, T? payload, ProblemDetails? problem, int status)> EvaluatePipelineAsync<T>(Task<Pipeline<T>> pipelineTask)
    {
        if (pipelineTask == null)
        {
            var pd = new ProblemDetails
            {
                Title = "Pipeline task cannot be null",
                Status = StatusCodes.Status400BadRequest
            };
            return (false, default, pd, StatusCodes.Status400BadRequest);
        }

        var pipeline = await pipelineTask.ConfigureAwait(false);
        if (pipeline == null)
        {
            var pd = new ProblemDetails
            {
                Title = "Result cannot be null",
                Status = StatusCodes.Status400BadRequest
            };
            return (false, default, pd, StatusCodes.Status400BadRequest);
        }

        var result = pipeline.Result;

        if (result.IsSuccess)
        {
            return (true, result.Value, null, StatusCodes.Status200OK);
        }

        var (pdMain, status) = BuildProblemDetailsFromErrors(result.Errors);
        return (false, default, pdMain, status);
    }

    public static async Task<Results<Ok<T>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> ToResultsOkAsync<T>(this Task<Pipeline<T>> pipelineTask) 
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask).ConfigureAwait(false);

        if (success)
        {
            return TypedResults.Ok(payload);
        }

        return status switch {
            StatusCodes.Status404NotFound => TypedResults.NotFound(problem!),
            StatusCodes.Status400BadRequest => TypedResults.BadRequest(problem!),
            _ => TypedResults.BadRequest(problem!)
        };
    }

    public static async Task<Results<Ok<T>, BadRequest<ProblemDetails>>> ToResultsSimpleOkAsync<T>(this Task<Pipeline<T>> pipelineTask) 
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask).ConfigureAwait(false);

        if (success)
        {
            return TypedResults.Ok(payload);
        }

        return status switch {
            StatusCodes.Status400BadRequest => TypedResults.BadRequest(problem!),
            _ => TypedResults.BadRequest(problem!)
        };
    }

    public static async Task<Results<Ok<T>, NotFound<ProblemDetails>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>> ToResultsOkExtendedAsync<T>(this Task<Pipeline<T>> pipelineTask)
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask).ConfigureAwait(false);

        if (success)
            return TypedResults.Ok(payload);

        return status switch
        {
            StatusCodes.Status404NotFound => TypedResults.NotFound(problem!),
            StatusCodes.Status409Conflict => TypedResults.Conflict(problem!),
            StatusCodes.Status400BadRequest => TypedResults.BadRequest(problem!),
            _ => TypedResults.BadRequest(problem!)
        };
    }

    public static async Task<Results<Created<T>, Conflict<ProblemDetails>, BadRequest<ProblemDetails>>> ToResultsCreatedAsync<T>(
        this Task<Pipeline<T>> pipelineTask,
        Func<T?, string>? locationFactory = null)
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask).ConfigureAwait(false);

        if (success)
        {
            var location = locationFactory?.Invoke(payload) ?? string.Empty;
            return TypedResults.Created(location, payload);
        }

        return status switch {
            StatusCodes.Status409Conflict => TypedResults.Conflict(problem!),
            StatusCodes.Status400BadRequest => TypedResults.BadRequest(problem!),
            _ => TypedResults.BadRequest(problem!)
        };
    }
}
