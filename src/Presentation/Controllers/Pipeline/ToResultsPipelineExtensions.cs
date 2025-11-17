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

    private static IResult CreateResult(Type resultType, object? payload, ProblemDetails? problem, string? location)
    {
        // OK / Created
        if (resultType.IsGenericType)
        {
            var generic = resultType.GetGenericTypeDefinition();
            var arg = resultType.GetGenericArguments()[0];

            if (generic == typeof(Ok<>))
                return TypedResults.Ok(payload);

            if (generic == typeof(Created<>))
                return TypedResults.Created(location ?? string.Empty, payload);

            if (generic == typeof(BadRequest<>))
                return TypedResults.BadRequest(problem);

            if (generic == typeof(NotFound<>))
                return TypedResults.NotFound(problem);

            if (generic == typeof(Conflict<>))
                return TypedResults.Conflict(problem);
        }

        throw new NotSupportedException($"Result type {resultType.Name} is not supported.");
    }

    private static async Task<IResult> ToResultsCoreAsync<TResponse>(
    Task<Pipeline<TResponse>> pipelineTask,
    Func<TResponse?, string>? locationFactory,
    params Type[] allowedResultTypes)
    {
        var (success, payload, problem, status) =
            await EvaluatePipelineAsync(pipelineTask);

        if (success)
        {
            var resultType = allowedResultTypes
                .First(t => t.Name.StartsWith("Ok") || t.Name.StartsWith("Created"));

            return CreateResult(resultType, payload, null, locationFactory?.Invoke(payload));
        }
        else
        {
            var resultType = allowedResultTypes.First(t =>
                status switch
                {
                    400 => t.Name.StartsWith("BadRequest"),
                    404 => t.Name.StartsWith("NotFound"),
                    409 => t.Name.StartsWith("Conflict"),
                    _ => false
                });

            return CreateResult(resultType, null, problem, null);
        }
    }

    // For Ok results (2 result types total)
    public static async Task<Results<Ok<TResponse>, TError1>> ToResultsAsync<TResponse, TError1>
    (
        this Task<Pipeline<TResponse>> pipelineTask
    )
    where TError1 : IResult
    {
        var result = await ToResultsCoreAsync(
            pipelineTask,
            null,
            typeof(Ok<TResponse>),
            typeof(TError1)
        );

        return (Results<Ok<TResponse>, TError1>)(object)result;
    }

    // For Ok results (3 result types total)
    public static async Task<Results<Ok<TResponse>, TError1, TError2>> ToResultsAsync<TResponse, TError1, TError2>
    (
        this Task<Pipeline<TResponse>> pipelineTask
    )
    where TError1 : IResult
    where TError2 : IResult
    {
        var result = await ToResultsCoreAsync(
            pipelineTask,
            null,
            typeof(Ok<TResponse>),
            typeof(TError1),
            typeof(TError2)
        );

        return (Results<Ok<TResponse>, TError1, TError2>)(object)result;
    }

    // For Ok results (4 result types total)
    public static async Task<Results<Ok<TResponse>, TError1, TError2, TError3>> ToResultsAsync<TResponse, TError1, TError2, TError3>
    (
        this Task<Pipeline<TResponse>> pipelineTask
    )
    where TError1 : IResult
    where TError2 : IResult
    where TError3 : IResult
    {
        var result = await ToResultsCoreAsync(
            pipelineTask,
            null,
            typeof(Ok<TResponse>),
            typeof(TError1),
            typeof(TError2),
            typeof(TError3)
        );

        return (Results<Ok<TResponse>, TError1, TError2, TError3>)(object)result;
    }

    // For Created results (2 result types total)
    public static async Task<Results<Created<TResponse>, TError1>> ToResultsCreatedAsync<TResponse, TError1>
    (
        this Task<Pipeline<TResponse>> pipelineTask,
        Func<TResponse?, string>? locationFactory = null
    )
    where TError1 : IResult
    {
        var result = await ToResultsCoreAsync(
            pipelineTask,
            locationFactory,
            typeof(Created<TResponse>),
            typeof(TError1)
        );

        return (Results<Created<TResponse>, TError1>)(object)result;
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
        var result = await ToResultsCoreAsync(
            pipelineTask,
            locationFactory,
            typeof(Created<TResponse>),
            typeof(TError1),
            typeof(TError2)
        );

        return (Results<Created<TResponse>, TError1, TError2>)(object)result;
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
        var result = await ToResultsCoreAsync(
            pipelineTask,
            locationFactory,
            typeof(Created<TResponse>),
            typeof(TError1),
            typeof(TError2),
            typeof(TError3)
        );

        return (Results<Created<TResponse>, TError1, TError2, TError3>)(object)result;
    }
}

