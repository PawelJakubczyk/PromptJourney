using Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Application.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    private const string ProblemDetailsContentType = "application/problem+json";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                NotFoundDomainException => (404, exception.Message),
                InvalidDomainException => (400, exception.Message),
                _ => (500, "An unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred while processing your request.",
                Detail = message,
                Status = context.Response.StatusCode
            };

            await context
                .Response
                .WriteAsJsonAsync(problemDetails, typeof(ProblemDetails), options: null, contentType: ProblemDetailsContentType);
        }
    }
}