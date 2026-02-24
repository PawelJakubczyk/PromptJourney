using Microsoft.AspNetCore.Http;

namespace Presentation.Constants;

/// <summary>
/// Provides HTTP status code priority mapping for error handling.
/// Lower values indicate higher priority (more urgent/critical).
/// </summary>
public static class StatusPriority
{
    /// <summary>
    /// Gets the priority value for a given HTTP status code.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <returns>Priority value (1 = highest priority, 9 = lowest priority).</returns>
    public static int GetPriority(int statusCode) => statusCode switch
    {
        // 5xx Server Errors (highest priority = 1)
        >= 500 and < 600 => 1,

        // 4xx Client Errors (priority 2-6 depending on type)
        StatusCodes.Status409Conflict => 2,           // State conflict - high priority
        StatusCodes.Status429TooManyRequests => 2,    // Throttling - high priority
        StatusCodes.Status404NotFound => 3,           // Not found - medium priority
        StatusCodes.Status401Unauthorized => 4,       // Auth errors
        StatusCodes.Status403Forbidden => 4,
        StatusCodes.Status408RequestTimeout => 4,
        StatusCodes.Status410Gone => 4,
        StatusCodes.Status402PaymentRequired => 6,    // Special case
        StatusCodes.Status418ImATeapot => 6,          // Special case (joke status)
        >= 400 and < 500 => 5,                        // Other 4xx client errors

        // 3xx Redirects (low priority = 7)
        >= 300 and < 400 => 7,

        // 2xx Success (very low priority = 8)
        >= 200 and < 300 => 8,

        // 1xx Information (lowest priority = 9)
        >= 100 and < 200 => 9,

        // Default for unknown codes
        _ => 10
    };

    /// <summary>
    /// Gets the priority value for a given HTTP status code with fallback to default.
    /// </summary>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="defaultPriority">The default priority if code is not recognized.</param>
    /// <returns>Priority value, or defaultPriority if not found.</returns>
    public static int GetPriorityOrDefault(int statusCode, int defaultPriority = int.MaxValue)
    {
        var priority = GetPriority(statusCode);
        return priority == int.MaxValue ? defaultPriority : priority;
    }
}