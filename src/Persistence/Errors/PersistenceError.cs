using FluentResults;

namespace Persistence.Errors;

public class PersistenceError : Error
{
    public PersistenceError(string message) : base(message)
    {
    }
}