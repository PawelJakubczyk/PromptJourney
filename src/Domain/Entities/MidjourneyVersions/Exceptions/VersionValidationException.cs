using Domain.Exceptions;

namespace Domain.Entities.MidjourneyVersions.Exceptions;

public sealed class VersionValidationException : MidjourneyEntitiesException
{
    public VersionValidationException(string fieldName, string message, string? context = null)
        : base($"VALIDATION_{fieldName.ToUpperInvariant()}", message, context)
    {
    }
}