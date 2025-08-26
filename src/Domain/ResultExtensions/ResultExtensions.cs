using FluentResults;

namespace Domain.ResultExtensions;

public static class ResultExtensions
{
    // Error type constants
    private const string ErrorType = "ErrorType";
    public const string NotFoundError = "NotFound";
    public const string InvalidError = "Invalid";
    public const string UnauthorizedError = "Unauthorized";
    public const string ForbiddenError = "Forbidden";
    public const string ConflictError = "Conflict";

    // 404 Not Found
    public static Result FailNotFound(this Result _, string errorMessage)
    {
        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, NotFoundError));
    }

    public static Result<T> FailNotFound<T>(this Result _, string errorMessage)
    {
        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, NotFoundError));
    }

    // 400 Bad Request
    public static Result FailInvalid(this Result _, string errorMessage)
    {
        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, InvalidError));
    }

    public static Result<T> FailInvalid<T>(this Result _, string errorMessage)
    {
        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, InvalidError));
    }

    // 401 Unauthorized
    public static Result FailUnauthorized(this Result _, string errorMessage)
    {
        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, UnauthorizedError));
    }

    public static Result<T> FailUnauthorized<T>(this Result _, string errorMessage)
    {
        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, UnauthorizedError));
    }

    // 403 Forbidden
    public static Result FailForbidden(this Result _, string errorMessage)
    {
        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, ForbiddenError));
    }

    public static Result<T> FailForbidden<T>(this Result _, string errorMessage)
    {
        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, ForbiddenError));
    }

    // 409 Conflict
    public static Result FailConflict(this Result _, string errorMessage)
    {
        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, ConflictError));
    }

    public static Result<T> FailConflict<T>(this Result _, string errorMessage)
    {
        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, ConflictError));
    }

    // Helper methods to check error type
    public static bool IsNotFound(this Error error) => 
        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == NotFoundError;

    public static bool IsInvalid(this Error error) => 
        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == InvalidError;

    public static bool IsUnauthorized(this Error error) => 
        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == UnauthorizedError;

    public static bool IsForbidden(this Error error) => 
        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == ForbiddenError;

    public static bool IsConflict(this Error error) => 
        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == ConflictError;
}
