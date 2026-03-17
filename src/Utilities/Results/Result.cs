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
}