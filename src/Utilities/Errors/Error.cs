namespace Utilities.Errors;

/// <summary>
/// Represents a custom error with message and metadata.
/// </summary>
public sealed class Error
{
    /// <summary>
    /// Gets the error message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the metadata dictionary containing additional error information.
    /// Key examples: "Layer", "ErrorCode", "Field", "ErrorCodeString", "RejectedValue"
    /// </summary>
    public Dictionary<string, object> Metadata { get; }

    /// <summary>
    /// Internal constructor to enforce creation through ErrorBuilder.
    /// </summary>
    public Error(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Metadata = [];
    }

    /// <summary>
    /// Creates a simple error with just a message.
    /// </summary>
    public static Error Simple(string message) => new(message);

    /// <summary>
    /// Creates an error from an exception.
    /// </summary>
    public static Error FromException(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        
        var error = new Error(exception.Message);
        error.Metadata["ExceptionType"] = exception.GetType().Name;
        
        if (exception.StackTrace is not null)
            error.Metadata["StackTrace"] = exception.StackTrace;
        
        if (exception.InnerException is not null)
            error.Metadata["InnerException"] = exception.InnerException.Message;
        
        return error;
    }

    /// <summary>
    /// Returns the error message.
    /// </summary>
    public override string ToString() => Message;

    /// <summary>
    /// Checks if two errors are equal based on message and metadata.
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Error other)
            return false;

        if (Message != other.Message)
            return false;

        if (Metadata.Count != other.Metadata.Count)
            return false;

        foreach (var kvp in Metadata)
        {
            if (!other.Metadata.TryGetValue(kvp.Key, out var value) || 
                !Equals(kvp.Value, value))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets the hash code based on message.
    /// </summary>
    public override int GetHashCode() => Message.GetHashCode();
}