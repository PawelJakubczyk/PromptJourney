namespace Utilities.Errors;

public class ErrorsMessages
{
    // General Message
    public const string ErrorBuilderBaseMessage = "An error occurred";
    public const string UnknownErrorMessage = "An Unknown error occurred";

    public const string NullPipelineTaskMessage = "Pipeline task cannot be null";
    public const string NullResultMessage = "Result cannot be null";

    // String Messages
    public static string NullOrWhitespaceMessage<TType>() => $"{typeof(TType).Name}: value cannot be null or whitespace.";
    public static string WhitespaceMessage<TType>() =>  $"{typeof(TType).Name}: value cannot be whitespace.";
    public static string TooLongMessage<TType>(string? value, int maxLength) => $"{typeof(TType).Name}: '{value}' cannot be longer than {maxLength} characters.";

    // Collection Messages
    public static string EmptyCollectionMessage<TType>() => $"{typeof(TType).Name}: cannot be empty.";
    public static string EmptyOrNullCollectionMessage<TType>() => $"{typeof(TType).Name}: cannot be null or empty.";
    public static string CollectionAlreadyContainsMessage<TType>(TType element) => $"{typeof(TType).Name}: collection already contains the element '{element}'.";
    public static string CollectionNotContainMessage<TType>(TType element) => $"{typeof(TType).Name}: collection does not contain the element '{element}'.";
    public static string CollectionNotContainMessage<TType>(List<TType> elements) => $"{typeof(TType).Name}: collection does not contain the elements '{string.Join(", ", elements)}'.";
    public static string DuplicateItemsMessage<TType>(List<TType> duplicates) => $"{typeof(TType).Name}: contains duplicates -> {string.Join(", ", duplicates)}.";

    // Entity Messages
    public static string NullMessage<TType>() => $"{typeof(TType).Name}: value cannot be null.";
    public static string NoAvailableExistMessage<TValue>() => $"No {typeof(TValue).Name} available";
    public static string NotFoundMessage<TEntity>(TEntity value) => $"{typeof(TEntity).Name} with value '{value}' not found";
    public static string AlreadyExistMessage<TEntity>(TEntity value) => $"{typeof(TEntity).Name} with value '{value}' already exist";

    // Format
    public static string InvalidPatternMessage<TType>(TType value, string patternDescription) => $"{typeof(TType).Name}: '{value}' does not match the required pattern ({patternDescription}).";

    // Enum Messages
    public static string OptionNotAllowedMessage<TType>(string value, Type enumType) => $"{typeof(TType).Name}: '{value}' is invalid. Expected values are: {string.Join(", ", Enum.GetNames(enumType))}.";

    // Database Messages
    public static string DatabaseConnectionFailedMessage(string? details = null) =>
    details is null
        ? "Database operation failed."
        : $"Database operation failed: {details}.";

    // Date Messages
    internal static string DateInFutureMessage(DateTime date) => $"Date '{date:yyyy-MM-dd}' cannot be in the future.";
    internal static string DateRangeNotChronologicalMessage(DateTime from, DateTime to) => $"Date range is not chronological: 'From' ({from:yyyy-MM-dd}) is after 'To' ({to:yyyy-MM-dd}).";
}
