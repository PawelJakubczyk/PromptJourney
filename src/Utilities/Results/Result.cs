using Utilities.Errors;

namespace Utilities.Results;

/// <summary>
/// Represents the result of an operation that can either succeed with a value or fail with errors.
/// </summary>
/// <typeparam name="T">The type of the value returned on success.</typeparam>
public sealed class Result<T>
{
    private readonly T? _value;
    private readonly List<Error> _errors;

    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailed => !IsSuccess;

    /// <summary>
    /// Gets the value of the result. Throws if the result is failed.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when accessing Value on a failed result.</exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException(
            $"Cannot access Value when Result is failed. Errors: {string.Join(", ", _errors.Select(e => e.Message))}");

    /// <summary>
    /// Gets the read-only collection of errors.
    /// </summary>
    public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

    // Private constructors to enforce factory pattern
    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        _errors = [];
    }

    private Result(List<Error> errors)
    {
        if (errors == null || errors.Count == 0)
            throw new ArgumentException("Result cannot be failed without errors.", nameof(errors));

        IsSuccess = false;
        _value = default;
        _errors = errors;
    }

    // ========================================
    // Factory Methods
    // ========================================

    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    public static Result<T> Ok(T value) => new(value);

    /// <summary>
    /// Creates a failed result with a single error.
    /// </summary>
    public static Result<T> Fail(Error error) => new([error]);

    /// <summary>
    /// Creates a failed result with multiple errors.
    /// </summary>
    public static Result<T> Fail(IEnumerable<Error> errors) => new([.. errors]);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result<T> Fail(string errorMessage) =>
        new([ErrorBuilder.New().WithMessage(errorMessage).Build()]);

    // ========================================
    // Implicit Conversions
    // ========================================

    public static implicit operator Result<T>(T value) => Ok(value);
    public static implicit operator Result<T>(Error error) => Fail(error);
    public static implicit operator Result<T>(List<Error> errors) => Fail(errors);
    public static implicit operator Result<T>(Error[] errors) => Fail(errors);

    // ========================================
    // Transformation Methods
    // ========================================

    /// <summary>
    /// Maps the result value to a new type if successful.
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        return IsSuccess ? Result<TNew>.Ok(mapper(Value)) : Result<TNew>.Fail(_errors);
    }

    /// <summary>
    /// Maps the result value asynchronously to a new type if successful.
    /// </summary>
    public async Task<Result<TNew>> MapAsync<TNew>(Func<T, Task<TNew>> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);
        return IsSuccess ? Result<TNew>.Ok(await mapper(Value)) : Result<TNew>.Fail(_errors);
    }

    /// <summary>
    /// Binds the result to another result-returning operation if successful.
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);
        return IsSuccess ? binder(Value) : Result<TNew>.Fail(_errors);
    }

    /// <summary>
    /// Binds the result asynchronously to another result-returning operation if successful.
    /// </summary>
    public async Task<Result<TNew>> BindAsync<TNew>(Func<T, Task<Result<TNew>>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);
        return IsSuccess ? await binder(Value) : Result<TNew>.Fail(_errors);
    }

    // ========================================
    // Pattern Matching
    // ========================================

    /// <summary>
    /// Pattern matches on the result, executing one of two functions based on success/failure.
    /// </summary>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<IReadOnlyList<Error>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccess ? onSuccess(Value) : onFailure(Errors);
    }

    /// <summary>
    /// Asynchronously pattern matches on the result.
    /// </summary>
    public async Task<TResult> MatchAsync<TResult>(
        Func<T, Task<TResult>> onSuccess,
        Func<IReadOnlyList<Error>, Task<TResult>> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccess ? await onSuccess(Value) : await onFailure(Errors);
    }

    // ========================================
    // Side Effects
    // ========================================

    /// <summary>
    /// Executes an action if the result is successful.
    /// </summary>
    public Result<T> OnSuccess(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (IsSuccess) action(Value);
        return this;
    }

    /// <summary>
    /// Executes an async action if the result is successful.
    /// </summary>
    public async Task<Result<T>> OnSuccessAsync(Func<T, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (IsSuccess) await action(Value);
        return this;
    }

    /// <summary>
    /// Executes an action if the result is failed.
    /// </summary>
    public Result<T> OnFailure(Action<IReadOnlyList<Error>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (IsFailed) action(Errors);
        return this;
    }

    /// <summary>
    /// Executes an async action if the result is failed.
    /// </summary>
    public async Task<Result<T>> OnFailureAsync(Func<IReadOnlyList<Error>, Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (IsFailed) await action(Errors);
        return this;
    }

    /// <summary>
    /// Executes an action regardless of success/failure (like finally).
    /// </summary>
    public Result<T> Tap(Action<Result<T>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        action(this);
        return this;
    }

    // ========================================
    // Utility Methods
    // ========================================

    /// <summary>
    /// Returns the value if successful, otherwise returns the default value.
    /// </summary>
    public T ValueOr(T defaultValue) => IsSuccess ? Value : defaultValue;

    /// <summary>
    /// Ensures the result satisfies a condition, failing with an error if not.
    /// </summary>
    public Result<T> Ensure(Func<T, bool> predicate, Error error)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);

        if (IsFailed)
            return this;

        return predicate(Value) ? this : Fail(error);
    }

    /// <summary>
    /// Ensures the result satisfies a condition, failing with an error message if not.
    /// </summary>
    public Result<T> Ensure(Func<T, bool> predicate, string errorMessage)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(errorMessage);

        return Ensure(predicate, ErrorBuilder.New().WithMessage(errorMessage).Build());
    }

    // ========================================
    // Combination Methods
    // ========================================

    /// <summary>
    /// Combines multiple results. Returns the first successful result, or all errors if all failed.
    /// </summary>
    public static Result<T> FirstSuccess(params Result<T>[] results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var firstSuccess = results.FirstOrDefault(r => r.IsSuccess);
        if (firstSuccess != null)
            return firstSuccess;

        var allErrors = results.SelectMany(r => r.Errors).ToList();
        return Fail(allErrors);
    }

    /// <summary>
    /// Combines multiple results. Succeeds only if all results succeed.
    /// </summary>
    public static Result<List<T>> Combine(params Result<T>[] results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var errors = results
            .Where(r => r.IsFailed)
            .SelectMany(r => r.Errors)
            .ToList();

        if (errors.Count > 0)
            return Result<List<T>>.Fail(errors);

        var values = results
            .Where(r => r.IsSuccess)
            .Select(r => r.Value)
            .ToList();

        return Result<List<T>>.Ok(values);
    }

    // ========================================
    // ToString Override
    // ========================================

    public override string ToString()
    {
        return IsSuccess
            ? $"Success: {Value}"
            : $"Failed: {string.Join(", ", _errors.Select(e => e.Message))}";
    }
}

/// <summary>
/// Represents the result of an operation that doesn't return a value.
/// </summary>
public sealed class Result
{
    private readonly List<Error> _errors;

    public bool IsSuccess { get; }
    public bool IsFailed => !IsSuccess;
    public IReadOnlyList<Error> Errors => _errors.AsReadOnly();

    private Result()
    {
        IsSuccess = true;
        _errors = [];
    }

    private Result(List<Error> errors)
    {
        if (errors == null || errors.Count == 0)
            throw new ArgumentException("Result cannot be failed without errors.", nameof(errors));

        IsSuccess = false;
        _errors = errors;
    }

    // ========================================
    // Factory Methods
    // ========================================

    public static Result Ok() => new();
    public static Result Fail(Error error) => new([error]);
    public static Result Fail(IEnumerable<Error> errors) => new([.. errors]);
    public static Result Fail(string errorMessage) =>
        new([ErrorBuilder.New().WithMessage(errorMessage).Build()]);

    // Generic Result creation
    public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);
    public static Result<T> Fail<T>(Error error) => Result<T>.Fail(error);
    public static Result<T> Fail<T>(IEnumerable<Error> errors) => Result<T>.Fail(errors);
    public static Result<T> Fail<T>(string errorMessage) => Result<T>.Fail(errorMessage);

    // ========================================
    // Implicit Conversions
    // ========================================

    public static implicit operator Result(Error error) => Fail(error);
    public static implicit operator Result(List<Error> errors) => Fail(errors);
    public static implicit operator Result(Error[] errors) => Fail(errors);

    // ========================================
    // Conversion Methods
    // ========================================

    public Result<T> ToResult<T>(T value) => IsSuccess ? Result<T>.Ok(value) : Result<T>.Fail(_errors);

    // ========================================
    // Pattern Matching
    // ========================================

    public TResult Match<TResult>(
        Func<TResult> onSuccess,
        Func<IReadOnlyList<Error>, TResult> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccess ? onSuccess() : onFailure(Errors);
    }

    public async Task<TResult> MatchAsync<TResult>(
        Func<Task<TResult>> onSuccess,
        Func<IReadOnlyList<Error>, Task<TResult>> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return IsSuccess ? await onSuccess() : await onFailure(Errors);
    }

    // ========================================
    // Side Effects
    // ========================================

    public Result OnSuccess(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (IsSuccess) action();
        return this;
    }

    public Result OnFailure(Action<IReadOnlyList<Error>> action)
    {
        ArgumentNullException.ThrowIfNull(action);
        if (IsFailed) action(Errors);
        return this;
    }

    // ========================================
    // ToString Override
    // ========================================

    public override string ToString()
    {
        return IsSuccess
            ? "Success"
            : $"Failed: {string.Join(", ", _errors.Select(e => e.Message))}";
    }
}