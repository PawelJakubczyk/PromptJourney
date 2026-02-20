using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Presentation.Models;
using Utilities.Constants;
using Utilities.Errors;

namespace Presentation.Controllers.Pipeline;

public static class ToResultsPipelineExtensions
{
    private static (ProblemDetails details, int status) BuildProblemDetailsFromErrors(
        IEnumerable<Error> errors,
        HttpContext? httpContext = null)
    {
        var list = errors.ToList();
        var mainError = list.Count != 0
            ? Pipeline<object>.PickHighestPriorityErrorInternal(list)
            : ErrorFactories.Unknown<PresentationLayer>();

        var status = mainError.GetErrorCode() ?? StatusCodes.Status500InternalServerError;
        
        var (title, detail, type) = status switch
        {
            400 => ("Validation failed", "One or more fields contain invalid values.", "https://api.promptjourney.com/errors/validation"),
            404 => (
                "Resource not found",
                list.FirstOrDefault()?.Message ?? "The requested resource could not be found.",
                "https://api.promptjourney.com/errors/not-found"
            ),
            409 => ("Conflict", GetConflictDetail(list), "https://api.promptjourney.com/errors/conflict"),
            500 => ("Internal server error", "An unexpected error occurred while processing your request.", "https://api.promptjourney.com/errors/internal"),
            _ => ("Error", "An error occurred while processing your request.", "https://api.promptjourney.com/errors/generic")
        };

        var requestInfo = GetRequestInfo(httpContext);

        if (status == 400)
        {
            var validationErrors = list
                .Select(e => new ValidationErrorDetail
                {
                    Field = e.GetField() ?? ExtractFieldNameFromMessage(e.Message),
                    Code = e.GetErrorCodeString() ?? "VALIDATION_ERROR",
                    Message = e.Message,
                    RejectedValue = e.GetRejectedValue()
                })
                .GroupBy(e => e.Field)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(err => new GroupedValidationErrorDetail
                    {
                        Code = err.Code,
                        Message = err.Message,
                        RejectedValue = err.RejectedValue
                    }).ToList()
                );

            var extendedPd = new ValidationProblemDetailsExtended
            {
                Type = type,
                Title = title,
                Detail = detail,
                Status = status,
                TraceId = httpContext?.TraceIdentifier,
                Request = requestInfo,
                Errors = validationErrors
            };

            return (extendedPd, status);
        }

        var pdMain = new ProblemDetails
        {
            Type = type,
            Title = title,
            Detail = detail,
            Status = status
        };

        if (httpContext != null)
        {
            pdMain.Extensions["traceId"] = httpContext.TraceIdentifier;
            pdMain.Extensions["request"] = requestInfo;
        }

        return (pdMain, status);
    }

    private static string ExtractFieldNameFromMessage(string message)
    {
        var colonIndex = message.IndexOf(':');
        
        if (colonIndex > 0)
        {
            var fieldName = message[..colonIndex].Trim();
            if (fieldName.Length > 0)
            {
                return char.ToLowerInvariant(fieldName[0]) + fieldName[1..];
            }
        }

        return "unknown";
    }

    private static async Task<(bool success, T? payload, ProblemDetails? problem, int status)> EvaluatePipelineAsync<T>(
        Task<Pipeline<T>> pipelineTask,
        HttpContext? httpContext = null)
    {
        if (pipelineTask == null)
        {
            var problemDetail = new ProblemDetails
            {
                Title = "Request pipeline error.",
                Detail = ErrorsMessages.NullPipelineTaskMessage,
                Status = StatusCodes.Status400BadRequest
            };
            return (false, default, problemDetail, StatusCodes.Status400BadRequest);
        }

        var pipeline = await pipelineTask.ConfigureAwait(false);

        if (pipeline == null)
        {
            var problemDetail = new ProblemDetails
            {
                Title = "Request pipeline error.",
                Detail = ErrorsMessages.NullResultMessage,
                Status = StatusCodes.Status400BadRequest
            };
            return (false, default, problemDetail, StatusCodes.Status400BadRequest);
        }

        var result = pipeline.Result;

        if (result.IsSuccess) return (true, result.Value, null, StatusCodes.Status200OK);

        var (pdMain, status) = BuildProblemDetailsFromErrors(result.Errors, httpContext);
        return (false, default, pdMain, status);
    }

    public static async Task<Results<Ok<TResponse>, TError1>> ToResultsOkAsync<TResponse, TError1>(
        this Task<Pipeline<TResponse>> pipelineTask,
        HttpContext? httpContext = null)
        where TError1 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask, httpContext);

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
        this Task<Pipeline<TResponse>> pipelineTask,
        HttpContext? httpContext = null
    )
    where TError1 : IResult
    where TError2 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask, httpContext);

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
        this Task<Pipeline<TResponse>> pipelineTask,
        HttpContext? httpContext = null
    )
    where TError1 : IResult
    where TError2 : IResult
    where TError3 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask, httpContext);

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
        Func<TResponse?, string>? locationFactory = null,
        HttpContext? httpContext = null
    )
    where TError1 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask, httpContext);

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
        Func<TResponse?, string>? locationFactory = null,
        HttpContext? httpContext = null
    )
    where TError1 : IResult
    where TError2 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask, httpContext);

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
        Func<TResponse?, string>? locationFactory = null,
        HttpContext? httpContext = null
    )
    where TError1 : IResult
    where TError2 : IResult
    where TError3 : IResult
    {
        var (success, payload, problem, status) = await EvaluatePipelineAsync(pipelineTask, httpContext);

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

    private static string GetConflictDetail(List<Error> errors)
    {
        if (errors.Count == 1)
        {
            return errors[0].Message;
        }
        
        return string.Join(" | ", errors.Select(e => e.Message));
    }

    /// <summary>
    /// Extracts request information (method and path template) from HttpContext.
    /// </summary>
    private static RequestInfo? GetRequestInfo(HttpContext? httpContext)
    {
        if (httpContext == null)
            return null;

        var method = httpContext.Request.Method;
        string path;

        // Get the endpoint metadata for route template
        var endpoint = httpContext.GetEndpoint();
        if (endpoint is RouteEndpoint routeEndpoint)
        {
            // Use route pattern (e.g., "api/versions/{version}/exists")
            path = routeEndpoint.RoutePattern.RawText ?? httpContext.Request.Path.Value ?? string.Empty;
        }
        else
        {
            // Fallback to actual request path
            path = httpContext.Request.Path.Value ?? string.Empty;
        }

        return new RequestInfo
        {
            Method = method,
            Path = path
        };
    }
}