using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using static Utilities.Errors.ErrorsMessages;

namespace Utilities.Errors;

public class ErrorFactories
{
    // General Errors
    public static Error Unknown<TLayer>()
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(UnknownErrorMessage)
            .WithErrorCode(StatusCodes.Status500InternalServerError)
            .Build();

    // String Errors
    public static Error NullOrWhitespace<TEntity, TLayer>()
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(NullOrWhitespaceMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error Whitespace<TEntity, TLayer>()
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(WhitespaceMessage<TEntity>())
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    public static Error TooLong<TEntity, TLayer>(string? value, int maxLength)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(TooLongMessage<TEntity>(value, maxLength))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    // Collection Errors
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

    // Entity Errors
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

    public static Error NotFound<TEntity, TLayer>(TEntity value)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(NotFoundMessage(value))
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    public static Error AlreadyExist<TEntity, TLayer>(TEntity value)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(AlreadyExistMessage(value))
            .WithErrorCode(StatusCodes.Status404NotFound)
            .Build();

    // Format Errors
    public static Error InvalidPattern<TEntity, TLayer>(TEntity value, string patternDescription)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(InvalidPatternMessage(value, patternDescription))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    // Enum Errors
    public static Error OptionNotAllowed<TEntity, TLayer>(string value, Type enumType)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(OptionNotAllowedMessage<TEntity>(value, enumType))
            .WithErrorCode(StatusCodes.Status400BadRequest)
            .Build();

    // Database Errors
    public static Error DatabaseConnectionFailed<TLayer>(string? details = null)
        where TLayer : ILayer =>
        ErrorBuilder.New()
            .WithLayer<TLayer>()
            .WithMessage(DatabaseConnectionFailedMessage(details))
            .WithErrorCode(StatusCodes.Status500InternalServerError)
            .Build();

    // Date Errors
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
}
