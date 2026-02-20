using Utilities.Errors;

namespace Utilities.Results;

public static class ResultExtensions
{
    // ========================================
    // Collection Extensions
    // ========================================

    /// <summary>
    /// Combines a collection of results into a single result containing a list of values.
    /// </summary>
    public static Result<List<T>> Combine<T>(this IEnumerable<Result<T>> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var resultsList = results.ToList();
        var errors = resultsList
            .Where(r => r.IsFailed)
            .SelectMany(r => r.Errors)
            .ToList();

        if (errors.Count > 0)
            return Result<List<T>>.Fail(errors);

        var values = resultsList
            .Where(r => r.IsSuccess)
            .Select(r => r.Value)
            .ToList();

        return Result<List<T>>.Ok(values);
    }

    /// <summary>
    /// Filters successful results from a collection.
    /// </summary>
    public static IEnumerable<T> WhereSuccess<T>(this IEnumerable<Result<T>> results)
    {
        ArgumentNullException.ThrowIfNull(results);
        return results.Where(r => r.IsSuccess).Select(r => r.Value);
    }

    /// <summary>
    /// Gets all errors from failed results in a collection.
    /// </summary>
    public static List<Error> GetAllErrors<T>(this IEnumerable<Result<T>> results)
    {
        ArgumentNullException.ThrowIfNull(results);
        return [.. results.Where(r => r.IsFailed).SelectMany(r => r.Errors)];
    }

    // ========================================
    // Async Extensions
    // ========================================

    /// <summary>
    /// Executes a side effect asynchronously without changing the result.
    /// </summary>
    public static async Task<Result<T>> TapAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, Task> action)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(action);

        var result = await resultTask;
        if (result.IsSuccess)
            await action(result.Value);
        return result;
    }

    /// <summary>
    /// Maps a task of result to a new type.
    /// </summary>
    public static async Task<Result<TNew>> MapAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, TNew> mapper)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(mapper);

        var result = await resultTask;
        return result.Map(mapper);
    }

    /// <summary>
    /// Binds a task of result to another result-returning operation.
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, Result<TNew>> binder)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(binder);

        var result = await resultTask;
        return result.Bind(binder);
    }

    /// <summary>
    /// Binds a task of result to another async result-returning operation.
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<TNew>>> binder)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(binder);

        var result = await resultTask;
        return await result.BindAsync(binder);
    }

    // ========================================
    // Validation Extensions
    // ========================================

    /// <summary>
    /// Ensures the result satisfies a condition asynchronously.
    /// </summary>
    public static async Task<Result<T>> EnsureAsync<T>(
        this Task<Result<T>> resultTask,
        Func<T, bool> predicate,
        Error error)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);

        var result = await resultTask;
        return result.Ensure(predicate, error);
    }

    /// <summary>
    /// Ensures the result satisfies an async predicate.
    /// </summary>
    public static async Task<Result<T>> EnsureAsync<T>(
        this Result<T> result,
        Func<T, Task<bool>> predicate,
        Error error)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);

        if (result.IsFailed)
            return result;

        var isValid = await predicate(result.Value);
        return isValid ? result : Result<T>.Fail(error);
    }

    // ========================================
    // Conversion Extensions
    // ========================================

    /// <summary>
    /// Converts a nullable value to a Result, failing if null.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : class
    {
        return value is not null ? Result<T>.Ok(value) : Result<T>.Fail(error);
    }

    /// <summary>
    /// Converts a nullable value to a Result, failing with a message if null.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, string errorMessage) where T : class
    {
        return value is not null
            ? Result<T>.Ok(value)
            : Result<T>.Fail(errorMessage);
    }

    /// <summary>
    /// Converts a nullable struct to a Result.
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : struct
    {
        return value.HasValue ? Result<T>.Ok(value.Value) : Result<T>.Fail(error);
    }

    // ========================================
    // Error Handling Extensions
    // ========================================

    /// <summary>
    /// Catches exceptions and converts them to failed results.
    /// </summary>
    public static Result<T> Try<T>(Func<T> action, Func<Exception, Error>? errorFactory = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            return Result<T>.Ok(action());
        }
        catch (Exception ex)
        {
            var error = errorFactory?.Invoke(ex) ??
                       ErrorBuilder.New()
                           .WithMessage($"An exception occurred: {ex.Message}")
                           .Build();
            return Result<T>.Fail(error);
        }
    }

    /// <summary>
    /// Catches exceptions asynchronously and converts them to failed results.
    /// </summary>
    public static async Task<Result<T>> TryAsync<T>(
        Func<Task<T>> action,
        Func<Exception, Error>? errorFactory = null)
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            var value = await action();
            return Result<T>.Ok(value);
        }
        catch (Exception ex)
        {
            var error = errorFactory?.Invoke(ex) ??
                       ErrorBuilder.New()
                           .WithMessage($"An exception occurred: {ex.Message}")
                           .Build();
            return Result<T>.Fail(error);
        }
    }

    public static List<TValue>? ToValueList<TValue>(this IEnumerable<Result<TValue>>? results)
    {
        if (results is null || !results.Any())
            return null;

        return [.. results
            .Where(result => result.IsSuccess && result.Value is not null)
            .Select(result => result.Value)];
    }
}