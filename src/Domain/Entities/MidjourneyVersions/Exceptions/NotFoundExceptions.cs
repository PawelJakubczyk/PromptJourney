using Domain.ErrorMassages;
using Domain.Exceptions;

namespace Domain.Entities.MidjourneyVersions.Exceptions;

public sealed class VersionNotFoundException : MidjourneyEntitiesException
{
    public VersionNotFoundException(string version, string? context = null)
        : base("VERSION_NOT_FOUND", ErrorMessages.VersionNotFound(version), context)
    {
    }
}

public sealed class ParameterNotFoundException : MidjourneyEntitiesException
{
    public ParameterNotFoundException(string parameterName, string version, string? context = null)
        : base("PARAMETER_NOT_FOUND", ErrorMessages.ParameterNotFound(parameterName, version), context)
    {
    }
}