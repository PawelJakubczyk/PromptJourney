using System.Text.Json.Serialization;

namespace Presentation.Models;

/// <summary>
/// Information about the HTTP request that caused the error.
/// </summary>
public class RequestInfo
{
    /// <summary>
    /// HTTP method (GET, POST, PUT, DELETE, PATCH, etc.).
    /// </summary>
    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    /// <summary>
    /// Request path template (e.g., "/api/versions/{version}/exists").
    /// </summary>
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;
}