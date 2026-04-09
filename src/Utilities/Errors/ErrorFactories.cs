using Microsoft.AspNetCore.Http;
using static Utilities.Errors.ErrorsMessages;

namespace Utilities.Errors;

public class ErrorFactories
{
    // ========================================
    // String Errors
    // ========================================

    /// <summary>
    /// Creates an error for null or whitespace values.
    /// </summary>
    public static Error NullOrWhitespace<TEntity>(string? value = null) =>
        ErrorBuilder.New()
            .WithMessage(NullOrWhitespaceMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("REQUIRED")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error for whitespace-only values.
    /// </summary>
    public static Error Whitespace<TEntity>(string? value = null) =>
        ErrorBuilder.New()
            .WithMessage(WhitespaceMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("REQUIRED")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error for values that are too long.
    /// </summary>
    public static Error TooLong<TEntity>(string? value, int maxLength) =>
        ErrorBuilder.New()
            .WithMessage(TooLongMessage<TEntity>(value, maxLength))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("TOO_LONG")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error for values that are too long.
    /// </summary>
    public static Error InvalidLength<TEntity>(string? value, int exactLength) =>
        ErrorBuilder.New()
            .WithMessage(MustHaveExactLengthMessage<TEntity>(value?.Length ?? 0, exactLength))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("TOO_LONG")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error for values that don't match the required pattern.
    /// </summary>
    public static Error InvalidPattern<TEntity>(string value, string patternDescription) =>
        ErrorBuilder.New()
            .WithMessage(InvalidPatternMessage(value, patternDescription))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("INVALID_FORMAT")
            .WithRejectedValue(value)
            .Build();

    // ========================================
    // Entity Errors
    // ========================================

    /// <summary>
    /// Creates an error when an entity is not found.
    /// </summary>
    public static Error NotFound<TEntity>(TEntity value) =>
        ErrorBuilder.New()
            .WithMessage(NotFoundMessage(value))
            .WithErrorCode(StatusCodes.Status404NotFound)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("NOT_FOUND")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error when an entity already exists.
    /// </summary>
    public static Error AlreadyExist<TEntity>(TEntity value) =>
        ErrorBuilder.New()
            .WithMessage(AlreadyExistMessage(value))
            .WithErrorCode(StatusCodes.Status409Conflict)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("ALREADY_EXISTS")
            .WithRejectedValue(value)
            .Build();

    public static Error Null<TEntity>() =>
        ErrorBuilder.New()
            .WithMessage(NullMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error NoAvailableExist<TEntity>() =>
        ErrorBuilder.New()
            .WithMessage(NoAvailableExistMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    // ========================================
    // General Errors
    // ========================================

    public static Error Unknown() =>
        ErrorBuilder.New()
            .WithMessage(UnknownErrorMessage)
            .WithErrorCode(StatusCodes.Status500InternalServerError)
            .Build();

    // ========================================
    // Collection Errors
    // ========================================

    public static Error Empty<TEntity>() =>
        ErrorBuilder.New()
            .WithMessage(EmptyCollectionMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error NullOrEmpty<TEntity>() =>
        ErrorBuilder.New()
            .WithMessage(EmptyOrNullCollectionMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error CollectionAlreadyContains<TElement>(TElement element) =>
        ErrorBuilder.New()
            .WithMessage(CollectionAlreadyContainsMessage(element))
            .WithErrorCode(StatusCodes.Status409Conflict)
            .Build();

    public static Error CollectionNotContain<TElement>(List<TElement> elements) =>
        ErrorBuilder.New()
            .WithMessage(CollectionNotContainMessage(elements))
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    public static Error DuplicateItems<TEntity>(List<TEntity> duplicates) =>
        ErrorBuilder.New()
            .WithMessage(DuplicateItemsMessage<TEntity>(duplicates))
            .WithErrorCode(StatusCodes.Status409Conflict)
            .Build();

    // ========================================
    // Numeric Errors
    // ========================================

    public static Error MustBeGreaterThanZero<TEntity>(int value) =>
        ErrorBuilder.New()
            .WithMessage(MustBeGreaterThanZeroMessage<TEntity>(value))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("INVALID_VALUE")
            .WithRejectedValue(value)
            .Build();

    public static Error ExceedsAvailable<TEntity>(int requested, int available) =>
        ErrorBuilder.New()
            .WithMessage(ExceedsAvailableMessage<TEntity>(requested, available))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("EXCEEDS_AVAILABLE")
            .WithRejectedValue(requested)
            .Build();

    // ========================================
    // Database Errors
    // ========================================

    public static Error DatabaseConnectionFailed(string? details = null) =>
        ErrorBuilder.New()
            .WithMessage(DatabaseConnectionFailedMessage(details))
            .WithErrorCode(StatusCodes.Status500InternalServerError)
            .Build();

    // ========================================
    // Date Errors
    // ========================================

    public static Error DateInFuture<TEntity>(DateTime date) =>
        ErrorBuilder.New()
            .WithMessage(DateInFutureMessage(date))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("INVALID_DATE")
            .WithRejectedValue(date)
            .Build();

    public static Error DateRangeNotChronological<TEntity>(DateTime from, DateTime to) =>
        ErrorBuilder.New()
            .WithMessage(DateRangeNotChronologicalMessage(from, to))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("INVALID_DATE_RANGE")
            .WithRejectedValue(from)
            .Build();

    /// <summary>
    /// Creates an error for values that don't match the required pattern.
    /// </summary>
    public static Error InvalidDateFormat<TEntity>(string value) =>
        ErrorBuilder.New()
            .WithMessage(DateFormatInvalidMessage(value))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("INVALID_DATE")
            .WithRejectedValue(value)
            .Build();

    // ========================================
    // Enum Errors
    // ========================================

    public static Error OptionNotAllowed<TEntity>(string value, Type enumType) =>
        ErrorBuilder.New()
            .WithMessage(OptionNotAllowedMessage<TEntity>(value, enumType))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("INVALID_OPTION")
            .WithRejectedValue(value)
            .Build();

    // ========================================
    // Security Errors
    // ========================================

    /// <summary>
    /// Creates an error for potentially malicious content (XSS, SQL injection patterns).
    /// </summary>
    public static Error SuspiciousContent<TEntity>(string value) =>
        ErrorBuilder.New()
            .WithMessage(SuspiciousContentMessage(value))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("FORBIDDEN_CONTENT")
            .WithRejectedValue(value)
            .Build();

    // ========================================
    // JSON/Deserialization Errors
    // ========================================

    public static Error InvalidJson(string? details = null) =>
        ErrorBuilder.New()
            .WithMessage(InvalidJsonMessage(details))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithErrorCodeString("INVALID_JSON")
            .Build();
}
