using Microsoft.AspNetCore.Mvc;
using Utilities.Constants;
using FluentResults;
using System.Text.Json;
using Utilities.Errors;

namespace App.Middleware;

internal sealed class GlobalExceptionHandlerMiddleware
(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger
)
{
    private const string _problemDetailsContentType = "application/problem+json";

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Error error = exception switch
        {
            ApplicationException appEx => ErrorBuilder.New()
                .WithLayer<ApplicationLayer>()
                .WithMessage(appEx.Message)
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build(),

            ArgumentException argEx => ErrorBuilder.New()
                .WithLayer<PersistenceLayer>()
                .WithMessage(argEx.Message)
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build(),

            InvalidOperationException invalidEx => ErrorBuilder.New()
                .WithLayer<PersistenceLayer>()
                .WithMessage(invalidEx.Message)
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build(),

            NotSupportedException notSupportedEx => ErrorBuilder.New()
                .WithLayer<PresentationLayer>()
                .WithMessage(notSupportedEx.Message)
                .WithErrorCode(StatusCodes.Status400BadRequest)
                .Build(),

            UnauthorizedAccessException unauthorizedEx => ErrorBuilder.New()
                .WithLayer<PresentationLayer>()
                .WithMessage(unauthorizedEx.Message)
                .WithErrorCode(StatusCodes.Status401Unauthorized)
                .Build(),

            FileNotFoundException fileNotFoundEx => ErrorBuilder.New()
                .WithLayer<InfrastructureLayer>()
                .WithMessage(fileNotFoundEx.Message)
                .WithErrorCode(StatusCodes.Status404NotFound)
                .Build(),

            DirectoryNotFoundException dirNotFoundEx => ErrorBuilder.New()
                .WithLayer<InfrastructureLayer>()
                .WithMessage(dirNotFoundEx.Message)
                .WithErrorCode(StatusCodes.Status404NotFound)
                .Build(),

            TimeoutException timeoutEx => ErrorBuilder.New()
                .WithLayer<PersistenceLayer>()
                .WithMessage(timeoutEx.Message)
                .WithErrorCode(StatusCodes.Status408RequestTimeout)
                .Build(),

            _ => ErrorBuilder.New()
                .WithLayer<PersistenceLayer>()
                .WithMessage("An unexpected error occurred")
                .WithErrorCode(StatusCodes.Status500InternalServerError)
                .Build()
        };

        var statusCode = error.GetErrorCode() ?? StatusCodes.Status500InternalServerError;
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = _problemDetailsContentType;

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Title = "An error occurred while processing your request",
            Detail = error.Message,
            Status = statusCode
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsJsonAsync(problemDetails, jsonOptions);
    }
}