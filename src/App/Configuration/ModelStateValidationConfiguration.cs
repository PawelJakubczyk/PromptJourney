using Microsoft.AspNetCore.Mvc;
using Presentation.Models;

namespace App.Configuration;

public static class ModelStateValidationConfiguration
{
    public static void ConfigureCustomModelStateValidation(this ApiBehaviorOptions options)
    {
        options.SuppressModelStateInvalidFilter = true;
        options.InvalidModelStateResponseFactory = CreateInvalidModelStateResponse;
    }

    private static IActionResult CreateInvalidModelStateResponse(ActionContext context)
    {
        var errors = context.ModelState
            .Where(e => e.Value?.Errors.Count > 0)
            .SelectMany(e => e.Value!.Errors.Select(error => new ValidationErrorDetail
            {
                Field = ToCamelCase(e.Key),
                Code = DetermineErrorCode(error.ErrorMessage),
                Message = error.ErrorMessage,
                RejectedValue = context.ModelState[e.Key]?.AttemptedValue
            }))
            .ToList();

        // Group errors by field
        var groupedErrors = errors
            .GroupBy(e => e.Field)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => new GroupedValidationErrorDetail
                {
                    Code = e.Code,
                    Message = e.Message,
                    RejectedValue = e.RejectedValue
                }).ToList()
            );

        var problemDetails = new ValidationProblemDetailsExtended
        {
            Type = "https://api.promptjourney.com/errors/validation",
            Title = "Validation failed",
            Status = 400,
            Detail = "One or more fields contain invalid values.",
            TraceId = context.HttpContext.TraceIdentifier,
            Errors = groupedErrors
        };

        return new BadRequestObjectResult(problemDetails)
        {
            ContentTypes = { "application/problem+json" }
        };
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    internal static string DetermineErrorCode(string errorMessage)
    {
        var lower = errorMessage.ToLowerInvariant();

        return lower switch
        {
            var msg when msg.Contains("required") => "REQUIRED",
            var msg when msg.Contains("range") || msg.Contains("between") => "OUT_OF_RANGE",
            var msg when msg.Contains("length") || msg.Contains("long") => "TOO_LONG",
            var msg when msg.Contains("format") || msg.Contains("invalid") => "INVALID_FORMAT",
            var msg when msg.Contains("email") => "INVALID_EMAIL",
            var msg when msg.Contains("url") => "INVALID_URL",
            _ => "VALIDATION_ERROR"
        };
    }
}