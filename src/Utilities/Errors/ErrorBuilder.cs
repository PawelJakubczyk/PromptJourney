namespace Utilities.Errors;

/// <summary>
/// Builder for creating Error instances with fluent API.
/// </summary>
public sealed class ErrorBuilder 
{
    private string _message = ErrorsMessages.ErrorBuilderBaseMessage;
    private int? _errorCode;
    private string? _fieldName;
    private string? _errorCodeString;
    private object? _rejectedValue;

    private ErrorBuilder() { }

    /// <summary>
    /// Creates a new ErrorBuilder instance.
    /// </summary>
    public static ErrorBuilder New() => new();

    /// <summary>
    /// Sets the HTTP status code for the error.
    /// </summary>
    public ErrorBuilder WithErrorCode(int? errorCode) 
    {
        _errorCode = errorCode;
        return this;
    }

    /// <summary>
    /// Sets the error message.
    /// </summary>
    public ErrorBuilder WithMessage(string message) 
    {
        _message = message ?? throw new ArgumentNullException(nameof(message));
        return this;
    }

    /// <summary>
    /// Sets the field name that caused the error.
    /// </summary>
    public ErrorBuilder WithField(string fieldName)
    {
        _fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
        return this;
    }

    /// <summary>
    /// Sets the error code as a string (e.g., "REQUIRED", "TOO_LONG").
    /// </summary>
    public ErrorBuilder WithErrorCodeString(string errorCode)
    {
        _errorCodeString = errorCode ?? throw new ArgumentNullException(nameof(errorCode));
        return this;
    }

    /// <summary>
    /// Sets the value that was rejected during validation.
    /// </summary>
    public ErrorBuilder WithRejectedValue(object? value)
    {
        _rejectedValue = value;
        return this;
    }

    /// <summary>
    /// Adds custom metadata to the error.
    /// </summary>
    public ErrorBuilder WithMetadata(string key, object value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);
        
        // Store for later addition in Build()
        if (!_customMetadata.ContainsKey(key))
            _customMetadata[key] = value;
        
        return this;
    }

    private readonly Dictionary<string, object> _customMetadata = [];

    /// <summary>
    /// Builds the Error instance with all specified properties.
    /// </summary>
    public Error Build() 
    {
        var error = new Error(_message);

        if (_errorCode is not null)
            error.Metadata["ErrorCode"] = _errorCode;

        if (_fieldName is not null)
            error.Metadata["Field"] = _fieldName;

        if (_errorCodeString is not null)
            error.Metadata["ErrorCodeString"] = _errorCodeString;

        error.Metadata["RejectedValue"] = _rejectedValue;

        // Add custom metadata
        foreach (var kvp in _customMetadata)
        {
            error.Metadata[kvp.Key] = kvp.Value;
        }

        return error;
    }
}

public static class ErrorExtensions
{
    public static int? GetErrorCode(this Error error)
    {
        if (error.Metadata.TryGetValue("ErrorCode", out var code) && code is int intCode)
            return intCode;

        return null;
    }

    public static string? GetField(this Error error)
    {
        if (error.Metadata.TryGetValue("Field", out var field) && field is string fieldStr)
            return fieldStr;

        return null;
    }

    public static string? GetErrorCodeString(this Error error)
    {
        if (error.Metadata.TryGetValue("ErrorCodeString", out var code) && code is string codeStr)
            return codeStr;

        return null;
    }

    public static object? GetRejectedValue(this Error error)
    {
        if (error.Metadata.TryGetValue("RejectedValue", out var value))
            return value;

        return null;
    }
}
