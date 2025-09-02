//using FluentResults;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System.Text.Json;

//namespace Application.Middlewares;

//public class ErrorHandlingMiddleware : IMiddleware
//{
//    private const string ProblemDetailsContentType = "application/problem+json";

//    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
//    {
//        try
//        {
//            await next.Invoke(context);
//        }
//        catch (Exception exception)
//        {
//            await HandleExceptionAsync(context, exception);
//        }
//    }

//    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
//    {
//        var statusCode = 500;
//        var message = "An unexpected error occurred.";

//        // Handle exceptions
//        if (exception is ResultException resultException)
//        {
//            (statusCode, message) = GetStatusCodeAndMessage(resultException.Result);
//        }
//        else
//        {
//            (statusCode, message) = exception switch
//            {
//                NotFoundDomainException => (404, exception.Message),
//                InvalidDomainException => (400, exception.Message),
//                _ => (500, "An unexpected error occurred.")
//            };
//        }

//        context.Response.StatusCode = statusCode;

//        var problemDetails = new ProblemDetails
//        {
//            Title = "An error occurred while processing your request.",
//            Detail = message,
//            Status = statusCode
//        };

//        context.Response.ContentType = ProblemDetailsContentType;
        
//        var jsonOptions = new JsonSerializerOptions
//        {
//            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//        };
        
//        await context.Response.WriteAsJsonAsync(problemDetails, jsonOptions);
//    }

//    private static (int statusCode, string message) GetStatusCodeAndMessage(ResultBase result)
//    {
//        if (result.IsSuccess)
//            return (200, string.Empty);

//        var errors = result.Errors;

//        if (errors.Any(e => e.IsNotFound()))
//            return (404, GetErrorMessage(errors));

//        if (errors.Any(e => e.IsInvalid()))
//            return (400, GetErrorMessage(errors));

//        if (errors.Any(e => e.IsUnauthorized()))
//            return (401, GetErrorMessage(errors));

//        if (errors.Any(e => e.IsForbidden()))
//            return (403, GetErrorMessage(errors));

//        if (errors.Any(e => e.IsConflict()))
//            return (409, GetErrorMessage(errors));

//        return (500, GetErrorMessage(errors));
//    }

//    private static string GetErrorMessage(List<IError> errors)
//    {
//        if (errors.Count == 0)
//            return "Unknown error";

//        if (errors.Count == 1)
//            return errors[0].Message;

//        return string.Join(" | ", errors.Select(e => e.Message));
//    }
//}

//// Exception types for backward compatibility
//public class NotFoundDomainException : Exception
//{
//    public NotFoundDomainException(string message) : base(message) { }
//}

//public class InvalidDomainException : Exception
//{
//    public InvalidDomainException(string message) : base(message) { }
//}

//// New exception to throw when using Result pattern
//public class ResultException : Exception
//{
//    public ResultBase Result { get; }

//    public ResultException(ResultBase result) : base("Operation resulted in error")
//    {
//        Result = result;
//    }
//}