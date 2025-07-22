namespace Domain.Exceptions;

public class MidjourneyEntitiesException : Exception
{
    public string ErrorCode { get; }
    public string? Context { get; }

    protected MidjourneyEntitiesException(string errorCode, string message, string? context = null) 
        : base(message)
    {
        ErrorCode = errorCode;
        Context = context;
    }
}