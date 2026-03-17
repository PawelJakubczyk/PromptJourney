using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Presentation.Models;

/// <summary>
/// Extended problem details for validation errors with structured error list.
/// </summary>
public class ValidationProblemDetailsExtended : ProblemDetails
{
    /// <summary>
    /// Trace identifier for debugging and correlation.
    /// </summary>
    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    /// <summary>
    /// Information about the HTTP request that caused the error.
    /// </summary>
    [JsonPropertyName("request")]
    public RequestInfo? Request { get; set; }

    /// <summary>
    /// List of validation errors with detailed information.
    /// </summary>
    [JsonPropertyName("errors")]
    public Dictionary<string, List<GroupedValidationErrorDetail>> Errors { get; set; } = [];
}

/// <summary>
/// Detailed validation error information.
/// </summary>
public class ValidationErrorDetail
{
    /// <summary>
    /// The field that failed validation (camelCase).
    /// </summary>
    [JsonPropertyName("field")]
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// Error code (e.g., REQUIRED, TOO_LONG, INVALID_FORMAT).
    /// </summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The value that was rejected during validation.
    /// </summary>
    [JsonPropertyName("rejectedValue")]
    public object? RejectedValue { get; set; }
}

public class GroupedValidationErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public object? RejectedValue { get; set; }
}