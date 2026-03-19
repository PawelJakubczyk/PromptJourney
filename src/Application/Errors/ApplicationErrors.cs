using Utilities.Errors;

namespace Application.Errors;

public static class ApplicationErrors
{
    public static Error HistoryRequestedCountMustBeGreaterThanZero(int count) =>
        ErrorFactories.MustBeGreaterThanZero<HistoryRequestedCount>(count);

    public static Error HistoryRequestedExceedsAvailableRecords(int requested, int available) =>
        ErrorFactories.ExceedsAvailable<HistoryRequestedCount>((requested), available);

    public static Error HistoryDateRangeNotChronological(DateTime from, DateTime to) =>
        ErrorFactories.DateRangeNotChronological<HistoryDateRange>(from, to);

    public static Error HistoryDateInFuture(DateTime date) =>
        ErrorFactories.DateInFuture<HistoryDate>(date);
}

// Marker record for error context
public sealed record HistoryRequestedCount;
public sealed record HistoryDateRange;
public sealed record HistoryDate;