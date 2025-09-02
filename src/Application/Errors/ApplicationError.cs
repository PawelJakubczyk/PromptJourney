using FluentResults;

namespace Application.Errors;

public static class ApplicationErrorMessages
{
    public class ApplicationError : Error
    {
        public ApplicationError(string message) : base(message)
        {
        }
    }
}

