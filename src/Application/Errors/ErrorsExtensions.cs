using Application.Abstractions.IRepository;
using Application.Features.Properties;
using Domain.ValueObjects;
using FluentResults;
using static Application.Errors.ApplicationErrorMessages;
using static Domain.Errors.DomainErrorMessages;

namespace Application.Errors;

public static class ErrorsExtensions
{
    public static Result<T>? CreateValidationErrorIfAny<T>
    (
        List<ApplicationError> applicationErrors,
        List<DomainError> domainErrors)
    {
        if (applicationErrors.Count == 0 && domainErrors.Count == 0)
            return null;

        var error = new Error("Validation failed");
        
        if (applicationErrors.Count > 0)
            error = error.WithMetadata("Application Errors", applicationErrors);
            
        if (domainErrors.Count > 0)
            error = error.WithMetadata("Domain Errors", domainErrors);

        return Result.Fail<T>(error);
    }

    public static Result<T>? CreateValidationErrorIfAny<T>
    (
        List<ApplicationError> applicationErrors,
        IReadOnlyList<IError> domainErrors)
    {
        if (applicationErrors.Count == 0 && domainErrors.Count == 0)
            return null;

        var error = new Error("Validation failed");
        
        if (applicationErrors.Count > 0)
            error = error.WithMetadata("Application Errors", applicationErrors);
            
        if (domainErrors.Count > 0)
            error = error.WithMetadata("Domain Errors", domainErrors);

        return Result.Fail<T>(error);
    }

    public static Result<T>? CreateValidationErrorIfAny<T>
    (
        List<DomainError> domainErrors)
    {
        if (domainErrors.Count == 0)
            return null;

        var error = new Error("Validation failed")
            .WithMetadata("Domain Errors", domainErrors);

        return Result.Fail<T>(error);
    }

    public static Result<T>? CreateValidationErrorIfAny<T>
    (
        List<ApplicationError> applicationErrors)
    {
        if (applicationErrors.Count == 0)
            return null;

        var error = new Error("Validation failed")
            .WithMetadata("Domain Errors", applicationErrors);

        return Result.Fail<T>(error);
    }

    public static List<ApplicationError> IfLinkNotExists(
        this List<ApplicationError> applicationErrors,
        ExampleLink link,
        IExampleLinksRepository repository
    )
    {
        var result = repository.CheckExampleLinkExistsAsync(link);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                $"Failed to check if example link '{link}' exists"));
        if (!result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Example link '{link}' does not exist"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfLinkAlreadyExists(
        this List<ApplicationError> applicationErrors,
        ExampleLink link,
        IExampleLinksRepository repository
    )
    {
        var result = repository.CheckExampleLinkExistsAsync(link);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                $"Failed to check if example link '{link}' exists"));
        if (result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Example link '{link}' already exists"));
        return applicationErrors;
    }
    public static List<ApplicationError> IfLinksEmpty(
        this List<ApplicationError> applicationErrors,
        IExampleLinksRepository repository
    )
    {
        var result = repository.CheckExampleLinksAreNotEmpty();
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                "Failed to check if example links have any element"));
        if (result.Result.Value) // true = pusta według oryginalnej metody
            applicationErrors.Add(new ApplicationError(
                "Example links have no elements"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfHistoryLimitNotGreaterThanZero(
        this List<ApplicationError> applicationErrors,
        int limit
    )
    {
        if (limit < 0)
            applicationErrors.Add(new ApplicationError(
                "The limit must be greater than zero."));
        return applicationErrors;
    }

    public static List<ApplicationError> IfHistoryCountExceedsAvailable(
        this List<ApplicationError> applicationErrors,
        int count,
        IPromptHistoryRepository repository
    )
    {
        var total = repository.CalculateHistoricalRecordCountAsync().Result.Value;
        if (count > total)
            applicationErrors.Add(new ApplicationError(
                $"The count ({count}) exceeds the number of historical records available."));
        return applicationErrors;
    }

    public static List<ApplicationError> IfDateInFuture(
        this List<ApplicationError> applicationErrors,
        DateTime inputDate
    )
    {
        if (inputDate > DateTime.UtcNow)
            applicationErrors.Add(new ApplicationError(
                "The provided date cannot be in the future."));
        return applicationErrors;
    }

    public static List<ApplicationError> IfDateRangeNotChronological(
        this List<ApplicationError> applicationErrors,
        DateTime startDate,
        DateTime endDate
    )
    {
        if (startDate >= endDate)
            applicationErrors.Add(new ApplicationError(
                $"Invalid date range: the start date ({startDate:yyyy-MM-dd}) must be earlier than the end date ({endDate:yyyy-MM-dd})."));
        return applicationErrors;
    }

    public static List<ApplicationError> IfPropertyAlreadyExists(
        this List<ApplicationError> applicationErrors,
        ModelVersion version,
        PropertyName propertyName,
        IPropertiesRepository repository
    )
    {
        var result = repository.CheckParameterExistsInVersionAsync(version, propertyName);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                $"Failed to check if parameter exists for version '{version}'"));
        if (result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Parameter '{propertyName}' already exists for version '{version}'"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfPropertyNotExists(
        this List<ApplicationError> applicationErrors,
        ModelVersion version,
        PropertyName propertyName,
        IPropertiesRepository repository
    )
    {
        var result = repository.CheckParameterExistsInVersionAsync(version, propertyName);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                $"Failed to check if parameter exists for version '{version}'"));
        if (!result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Parameter '{propertyName}' does not exist for version '{version}'"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfNoPropertyDetailsFound(
        this List<ApplicationError> applicationErrors,
        string input
    )
    {
        var propertyNames = typeof(PropertyDetails)
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
            .Select(p => p.Name);
        if (propertyNames == null || !propertyNames.Any())
            applicationErrors.Add(new ApplicationError(
                "No public instance properties found in PropertyDetails."));
        return applicationErrors;
    }

    public static List<ApplicationError> IfStyleNotExists(
        this List<ApplicationError> applicationErrors,
        StyleName styleName,
        IStyleRepository repository
    )
    {
        var result = repository.CheckStyleExistsAsync(styleName);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                $"Failed to check if style '{styleName}' exists"));
        if (!result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Style '{styleName}' does not exist"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfStyleAlreadyExists(
        this List<ApplicationError> applicationErrors,
        StyleName styleName,
        IStyleRepository repository
    )
    {
        var result = repository.CheckStyleExistsAsync(styleName);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                $"Failed to check if style '{styleName}' exists"));
        if (result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Style '{styleName}' already exists"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfTagNotExists(
        this List<ApplicationError> applicationErrors,
        StyleName styleName,
        Tag tag,
        IStyleRepository repository
    )
    {
        var result = repository.CheckTagExistsInStyleAsync(styleName, tag);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                $"Failed to check if tag '{tag}' for style '{styleName}' exists"));
        if (!result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Tag '{tag}' for style '{styleName}' does not exist"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfTagAlreadyExists(
        this List<ApplicationError> applicationErrors,
        StyleName styleName,
        Tag tag,
        IStyleRepository repository
    )
    {
        var result = repository.CheckTagExistsInStyleAsync(styleName, tag);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError(
                $"Failed to check if tag '{tag}' for style '{styleName}' exists"));
        if (result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Tag '{tag}' for style '{styleName}' already exists"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfVersionNotExists(
        this List<ApplicationError> applicationErrors,
        ModelVersion version,
        IVersionRepository repository
    )
    {
        var result = repository.CheckVersionExistsInVersionsAsync(version);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError("Failed to check if version exists"));
        if (!result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Version '{version}' not found"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfVersionAlreadyExists(
        this List<ApplicationError> applicationErrors,
        ModelVersion version,
        IVersionRepository repository
    )
    {
        var result = repository.CheckVersionExistsInVersionsAsync(version);
        if (result.Result.IsFailed)
            applicationErrors.Add(new ApplicationError("Failed to check if version exists"));
        if (result.Result.Value)
            applicationErrors.Add(new ApplicationError(
                $"Version '{version}' already exist"));
        return applicationErrors;
    }

    public static List<ApplicationError> IfNoSupportedVersions(
        this List<ApplicationError> applicationErrors,
        IVersionRepository repository
    )
    {
        var result = repository.CheckIfAnySupportedVersionExistsAsync();
        if (result.Result.Value)
            applicationErrors.Add(new ApplicationError("No supported versions found."));
        return applicationErrors;
    }

    public static List<ApplicationError> CollectErrors<T>(
        this List<ApplicationError> errors,
        Result<T>? result
    )
    {
        if (result is not null && result.IsFailed)
        {
            errors.AddRange(
                result.Errors.OfType<ApplicationError>()
            );
        }

        return errors;
    }
}