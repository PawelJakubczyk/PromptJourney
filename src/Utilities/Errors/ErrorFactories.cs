using Microsoft.AspNetCore.Http;
using Utilities.Constants;
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
    public static Error NullOrWhitespace<TEntity, TLayer>(string? value = null)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(NullOrWhitespaceMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("REQUIRED")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error for whitespace-only values.
    /// </summary>
    public static Error Whitespace<TEntity, TLayer>(string? value = null)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(WhitespaceMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("REQUIRED")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error for values that are too long.
    /// </summary>
    public static Error TooLong<TEntity, TLayer>(string? value, int maxLength)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(TooLongMessage<TEntity>(value, maxLength))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("TOO_LONG")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error for values that don't match the required pattern.
    /// </summary>
    public static Error InvalidPattern<TEntity, TLayer>(string value, string patternDescription)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
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
    public static Error NotFound<TEntity, TLayer>(TEntity value)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(NotFoundMessage(value))
            .WithErrorCode(StatusCodes.Status404NotFound)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("NOT_FOUND")
            .WithRejectedValue(value)
            .Build();

    /// <summary>
    /// Creates an error when an entity already exists.
    /// </summary>
    public static Error AlreadyExist<TEntity, TLayer>(TEntity value)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(AlreadyExistMessage(value))
            .WithErrorCode(StatusCodes.Status409Conflict)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("ALREADY_EXISTS")
            .WithRejectedValue(value)
            .Build();

    // ========================================
    // General Errors
    // ========================================

    public static Error Unknown<TLayer>()
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(UnknownErrorMessage)
            .WithErrorCode(StatusCodes.Status500InternalServerError)
            .Build();

    // ========================================
    // Collection Errors
    // ========================================

    public static Error Empty<TEntity, TLayer>()
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(EmptyCollectionMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error NullOrEmpty<TEntity, TLayer>()
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(EmptyOrNullCollectionMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error CollectionAlreadyContains<TElement, TLayer>(TElement element)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(CollectionAlreadyContainsMessage(element))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error CollectionNotContain<TElement, TLayer>(List<TElement> elements)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(CollectionNotContainMessage(elements))
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    public static Error DuplicateItems<TEntity, TLayer>(List<TEntity> duplicates)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(DuplicateItemsMessage<TEntity>(duplicates))
            .WithErrorCode(StatusCodes.Status409Conflict)
            .Build();

    // ========================================
    // Entity Errors
    // ========================================
    public static Error Null<TEntity, TLayer>()
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(NullMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error NoAvailableExist<TEntity, TLayer>()
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(NoAvailableExistMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    // ========================================
    // Database Errors
    // ========================================

    public static Error DatabaseConnectionFailed<TLayer>(string? details = null)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(DatabaseConnectionFailedMessage(details))
            .WithErrorCode(StatusCodes.Status500InternalServerError)
            .Build();

    // ========================================
    // Date Errors
    // ========================================

    public static Error DateInFuture(DateTime date) =>
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage(DateInFutureMessage(date))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error DateRangeNotChronological(DateTime from, DateTime to) =>
        ErrorBuilder.New()
            .WithLayer<DomainLayer>()
            .WithMessage(DateRangeNotChronologicalMessage(from, to))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    /// <summary>
    /// Creates an error for values that don't match the required pattern.
    /// </summary>
    public static Error InvalidDateFormat<TEntity, TLayer>(string value)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(DateFormatInvalidMessage(value))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("INVALID_DATE")
            .WithRejectedValue(value)
            .Build();

    // ========================================
    // Enum Errors
    // ========================================

    public static Error OptionNotAllowed<TEntity, TLayer>(string value, Type enumType)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(OptionNotAllowedMessage<TEntity>(value, enumType))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .WithField(typeof(TEntity).Name.ToLowerInvariant())
            .WithErrorCodeString("INVALID_OPTION")
            .WithRejectedValue(value)
            .Build();
}
