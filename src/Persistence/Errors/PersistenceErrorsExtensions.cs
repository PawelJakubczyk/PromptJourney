using FluentResults;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using Domain.ValueObjects;

namespace Persistence.Errors;

public static class PersistenceErrorsExtensions
{
    public static List<PersistenceError> If(this List<PersistenceError> domainErrors, bool condition, PersistenceError errorIfTrue)
    {
        if (condition)
        {
            domainErrors.Add(errorIfTrue);
        }

        return domainErrors;
    }

    public static List<PersistenceError> IfStyleNotExists(this List<PersistenceError> persistenceErrors, StyleName styleName)
    {
        // Note: This method should be implemented with database context injection or passed as parameter
        // For now, adding a placeholder validation
        if (string.IsNullOrWhiteSpace(styleName?.Value))
        {
            persistenceErrors.Add(new PersistenceError($"StyleName cannot be null or empty"));
        }

        return persistenceErrors;
    }

    public static List<PersistenceError> IfVersionNotExists(this List<PersistenceError> persistenceErrors, ModelVersion version)
    {
        // Note: This method should be implemented with database context injection or passed as parameter
        // For now, adding a placeholder validation
        if (string.IsNullOrWhiteSpace(version?.Value))
        {
            persistenceErrors.Add(new PersistenceError($"ModelVersion cannot be null or empty"));
        }

        return persistenceErrors;
    }

    public static Result<T>? CreateValidationErrorIfAny<T>(List<PersistenceError> persistenceErrors)
    {
        if (persistenceErrors.Count == 0)
            return null;

        var error = new Error("Validation failed")
            .WithMetadata("Persistence Errors", persistenceErrors);

        return Result.Fail<T>(error);
    }
}