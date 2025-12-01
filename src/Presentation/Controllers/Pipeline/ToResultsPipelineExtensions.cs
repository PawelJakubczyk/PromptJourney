using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using Utilities.Errors;

namespace Presentation.Controllers.Pipeline;

public static class ToResultsPipelineExtensions
{
    private static (ProblemDetails details, int status) BuildProblemDetailsFromErrors(IEnumerable<IError> errors)
    {
        var list = errors.OfType<Error>().ToList();
        var mainError = list.Count != 0
            ? Pipeline<object>.PickHighestPriorityErrorInternal(list)
            : ErrorFactories.Unknown<PresentationLayer>();

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
            var problemDetail = new ProblemDetails
            {
                Title = ErrorsMessages.NullPipelineTaskMessage,
                Status = StatusCodes.Status400BadRequest
            };
            return (false, default, problemDetail, StatusCodes.Status400BadRequest);
        }

        var pipeline = await pipelineTask.ConfigureAwait(false);

        if (pipeline == null)
        {
            var problemDetail = new ProblemDetails
            {
                Title = ErrorsMessages.NullResultMessage,
                Status = StatusCodes.Status400BadRequest
            };
            return (false, default, problemDetail, StatusCodes.Status400BadRequest);
        }

        var result = pipeline.Result;

        if (result.IsSuccess) return (true, result.Value, null, StatusCodes.Status200OK);

        var (pdMain, status) = BuildProblemDetailsFromErrors(result.Errors);
        return (false, default, pdMain, status);
    }

    // For Ok results (2 result types total)
    public static async Task<Results<Ok<TResponse>, TError1>> ToResultsOkAsync<TResponse, TError1>
    (
        this Task<Pipeline<TResponse>> pipelineTask
    )
    where TError1 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask);

        if (success)
        {
            return TypedResults.Ok(payload!);
        }

        if (status == 400 && typeof(TError1) == typeof(BadRequest<ProblemDetails>))
            return (TError1)(object)TypedResults.BadRequest(problem);
        if (status == 404 && typeof(TError1) == typeof(NotFound<ProblemDetails>))
            return (TError1)(object)TypedResults.NotFound(problem);
        if (status == 409 && typeof(TError1) == typeof(Conflict<ProblemDetails>))
            return (TError1)(object)TypedResults.Conflict(problem);

        return (TError1)(object)TypedResults.BadRequest(problem);
    }

    // For Ok results (3 result types total)
    public static async Task<Results<Ok<TResponse>, TError1, TError2>> ToResultsOkAsync<TResponse, TError1, TError2>
    (
        this Task<Pipeline<TResponse>> pipelineTask
    )
    where TError1 : IResult
    where TError2 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask);

        if (success)
        {
            return TypedResults.Ok(payload!);
        }

        if (status == 400)
        {
            if (typeof(TError1) == typeof(BadRequest<ProblemDetails>)) return (TError1)(object)TypedResults.BadRequest(problem);
            if (typeof(TError2) == typeof(BadRequest<ProblemDetails>)) return (TError2)(object)TypedResults.BadRequest(problem);
        }
        else if (status == 404)
        {
            if (typeof(TError1) == typeof(NotFound<ProblemDetails>)) return (TError1)(object)TypedResults.NotFound(problem);
            if (typeof(TError2) == typeof(NotFound<ProblemDetails>)) return (TError2)(object)TypedResults.NotFound(problem);
        }
        else if (status == 409)
        {
            if (typeof(TError1) == typeof(Conflict<ProblemDetails>)) return (TError1)(object)TypedResults.Conflict(problem);
            if (typeof(TError2) == typeof(Conflict<ProblemDetails>)) return (TError2)(object)TypedResults.Conflict(problem);
        }

        // Fallback to BadRequest
        if (typeof(TError1) == typeof(BadRequest<ProblemDetails>)) return (TError1)(object)TypedResults.BadRequest(problem);
        if (typeof(TError2) == typeof(BadRequest<ProblemDetails>)) return (TError2)(object)TypedResults.BadRequest(problem);

        throw new InvalidOperationException($"No suitable error result type found for status code {status}");
    }

    // For Ok results (4 result types total)
    public static async Task<Results<Ok<TResponse>, TError1, TError2, TError3>> ToResultsOkAsync<TResponse, TError1, TError2, TError3>
    (
        this Task<Pipeline<TResponse>> pipelineTask
    )
    where TError1 : IResult
    where TError2 : IResult
    where TError3 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask);

        if (success)
        {
            return TypedResults.Ok(payload!);
        }

        if (status == 400)
        {
            if (typeof(TError1) == typeof(BadRequest<ProblemDetails>)) return (TError1)(object)TypedResults.BadRequest(problem);
            if (typeof(TError2) == typeof(BadRequest<ProblemDetails>)) return (TError2)(object)TypedResults.BadRequest(problem);
            if (typeof(TError3) == typeof(BadRequest<ProblemDetails>)) return (TError3)(object)TypedResults.BadRequest(problem);
        }
        else if (status == 404)
        {
            if (typeof(TError1) == typeof(NotFound<ProblemDetails>)) return (TError1)(object)TypedResults.NotFound(problem);
            if (typeof(TError2) == typeof(NotFound<ProblemDetails>)) return (TError2)(object)TypedResults.NotFound(problem);
            if (typeof(TError3) == typeof(NotFound<ProblemDetails>)) return (TError3)(object)TypedResults.NotFound(problem);
        }
        else if (status == 409)
        {
            if (typeof(TError1) == typeof(Conflict<ProblemDetails>)) return (TError1)(object)TypedResults.Conflict(problem);
            if (typeof(TError2) == typeof(Conflict<ProblemDetails>)) return (TError2)(object)TypedResults.Conflict(problem);
            if (typeof(TError3) == typeof(Conflict<ProblemDetails>)) return (TError3)(object)TypedResults.Conflict(problem);
        }

        // Fallback to BadRequest
        if (typeof(TError1) == typeof(BadRequest<ProblemDetails>)) return (TError1)(object)TypedResults.BadRequest(problem);
        if (typeof(TError2) == typeof(BadRequest<ProblemDetails>)) return (TError2)(object)TypedResults.BadRequest(problem);
        if (typeof(TError3) == typeof(BadRequest<ProblemDetails>)) return (TError3)(object)TypedResults.BadRequest(problem);

        throw new InvalidOperationException($"No suitable error result type found for status code {status}");
    }

    // For Created results (2 result types total)
    public static async Task<Results<Created<TResponse>, TError1>> ToResultsCreatedAsync<TResponse, TError1>
    (
        this Task<Pipeline<TResponse>> pipelineTask,
        Func<TResponse?, string>? locationFactory = null
    )
    where TError1 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask);

        if (success)
        {
            var location = locationFactory?.Invoke(payload) ?? string.Empty;
            return TypedResults.Created(location, payload!);
        }

        if (status == 400 && typeof(TError1) == typeof(BadRequest<ProblemDetails>))
            return (TError1)(object)TypedResults.BadRequest(problem);
        if (status == 404 && typeof(TError1) == typeof(NotFound<ProblemDetails>))
            return (TError1)(object)TypedResults.NotFound(problem);
        if (status == 409 && typeof(TError1) == typeof(Conflict<ProblemDetails>))
            return (TError1)(object)TypedResults.Conflict(problem);

        return (TError1)(object)TypedResults.BadRequest(problem);
    }

    // For Created results (3 result types total)
    public static async Task<Results<Created<TResponse>, TError1, TError2>> ToResultsCreatedAsync<TResponse, TError1, TError2>
    (
        this Task<Pipeline<TResponse>> pipelineTask,
        Func<TResponse?, string>? locationFactory = null
    )
    where TError1 : IResult
    where TError2 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask);

        if (success)
        {
            var location = locationFactory?.Invoke(payload) ?? string.Empty;
            return TypedResults.Created(location, payload!);
        }

        if (status == 400)
        {
            if (typeof(TError1) == typeof(BadRequest<ProblemDetails>)) return (TError1)(object)TypedResults.BadRequest(problem);
            if (typeof(TError2) == typeof(BadRequest<ProblemDetails>)) return (TError2)(object)TypedResults.BadRequest(problem);
        }
        else if (status == 404)
        {
            if (typeof(TError1) == typeof(NotFound<ProblemDetails>)) return (TError1)(object)TypedResults.NotFound(problem);
            if (typeof(TError2) == typeof(NotFound<ProblemDetails>)) return (TError2)(object)TypedResults.NotFound(problem);
        }
        else if (status == 409)
        {
            if (typeof(TError1) == typeof(Conflict<ProblemDetails>)) return (TError1)(object)TypedResults.Conflict(problem);
            if (typeof(TError2) == typeof(Conflict<ProblemDetails>)) return (TError2)(object)TypedResults.Conflict(problem);
        }

        // Fallback to BadRequest
        if (typeof(TError1) == typeof(BadRequest<ProblemDetails>)) return (TError1)(object)TypedResults.BadRequest(problem);
        if (typeof(TError2) == typeof(BadRequest<ProblemDetails>)) return (TError2)(object)TypedResults.BadRequest(problem);

        throw new InvalidOperationException($"No suitable error result type found for status code {status}");
    }

    // For Created results (4 result types total)
    public static async Task<Results<Created<TResponse>, TError1, TError2, TError3>> ToResultsCreatedAsync<TResponse, TError1, TError2, TError3>
    (
        this Task<Pipeline<TResponse>> pipelineTask,
        Func<TResponse?, string>? locationFactory = null
    )
    where TError1 : IResult
    where TError2 : IResult
    where TError3 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask);

        if (success)
        {
            var location = locationFactory?.Invoke(payload) ?? string.Empty;
            return TypedResults.Created(location, payload!);
        }

        if (status == 400)
        {
            if (typeof(TError1) == typeof(BadRequest<ProblemDetails>)) return (TError1)(object)TypedResults.BadRequest(problem);
            if (typeof(TError2) == typeof(BadRequest<ProblemDetails>)) return (TError2)(object)TypedResults.BadRequest(problem);
            if (typeof(TError3) == typeof(BadRequest<ProblemDetails>)) return (TError3)(object)TypedResults.BadRequest(problem);
        }
        else if (status == 404)
        {
            if (typeof(TError1) == typeof(NotFound<ProblemDetails>)) return (TError1)(object)TypedResults.NotFound(problem);
            if (typeof(TError2) == typeof(NotFound<ProblemDetails>)) return (TError2)(object)TypedResults.NotFound(problem);
            if (typeof(TError3) == typeof(NotFound<ProblemDetails>)) return (TError3)(object)TypedResults.NotFound(problem);
        }
        else if (status == 409)
        {
            if (typeof(TError1) == typeof(Conflict<ProblemDetails>)) return (TError1)(object)TypedResults.Conflict(problem);
            if (typeof(TError2) == typeof(Conflict<ProblemDetails>)) return (TError2)(object)TypedResults.Conflict(problem);
            if (typeof(TError3) == typeof(Conflict<ProblemDetails>)) return (TError3)(object)TypedResults.Conflict(problem);
        }

        // Fallback to BadRequest
        if (typeof(TError1) == typeof(BadRequest<ProblemDetails>)) return (TError1)(object)TypedResults.BadRequest(problem);
        if (typeof(TError2) == typeof(BadRequest<ProblemDetails>)) return (TError2)(object)TypedResults.BadRequest(problem);
        if (typeof(TError3) == typeof(BadRequest<ProblemDetails>)) return (TError3)(object)TypedResults.BadRequest(problem);

        throw new InvalidOperationException($"No suitable error result type found for status code {status}");
    }
}