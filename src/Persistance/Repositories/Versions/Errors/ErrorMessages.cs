namespace Persistance.Repositories.Versions.ErrorMassages;

public static class ErrorMessages
{

    // Request validation messages
    public const string RequestBodyNull = "Request body cannot be null";
    public const string VersionParameterNullOrEmpty = "Version parameter cannot be null or empty";
    public const string PropertyNameParameterNullOrEmpty = "PropertyName parameter cannot be null or empty";

    // Not found messages
    public static string VersionNotFound(string version) => $"Version '{version}' not found";
    public static string ParameterNotFound(string parameterName, string version) => $"Parameter '{parameterName}' not found in version '{version}'";

    // Database messages
    public static string DatabaseError(string operation) => $"Database error while {operation}";
    public static string DatabaseErrorWithVersion(string operation, string version) => $"Database error while {operation} for version '{version}'";
}