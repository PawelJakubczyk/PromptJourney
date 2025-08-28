//using FluentResults;
//using static Domain.Errors.DomainErrorMessages;
//using static System.Runtime.InteropServices.JavaScript.JSType;

//namespace Domain.ResultExtensions;



//public class MyResult : Result
//{
//    private const string ErrorType = "ErrorType";
//    public const string NotFoundError = "NotFound";
//    public const string InvalidError = "Invalid";
//    public const string UnauthorizedError = "Unauthorized";
//    public const string ForbiddenError = "Forbidden";
//    public const string ConflictError = "Conflict";

//    public static Result FailNotFound(string errorMessage)
//    {
//        return Fail(new Error(errorMessage).WithMetadata(ErrorType, NotFoundError));
//    }

//    public static void DFDFD()
//    {
//        var result = MyResult.FailNotFound("Resource not found");
//    }
//}

//public class MyError : Error
//{
//    private const string ErrorType = "ErrorType";
//    public const string NotFoundError = "NotFound";
//    public const string InvalidError = "Invalid";
//    public const string UnauthorizedError = "Unauthorized";
//    public const string ForbiddenError = "Forbidden";
//    public const string ConflictError = "Conflict";

//    public static MyError NotFound = (MyError)(new Error("Not Found").WithMetadata(ErrorType, NotFoundError));

//    //to generyczne
//    public MyResult ToResult()
//    {
//        return (MyResult)Result.Fail(NotFound);
//    }
//}

//public static class MYCOS
//{
//    public static void IsNotFound()
//    {
//        var result = MyError.NotFound.ToResult();
//    }
//}

//public static class ResultExtensions
//{
//    // Error type constants
//    private const string ErrorType = "ErrorType";
//    public const string NotFoundError = "NotFound";
//    public const string InvalidError = "Invalid";
//    public const string UnauthorizedError = "Unauthorized";
//    public const string ForbiddenError = "Forbidden";
//    public const string ConflictError = "Conflict";

//    // 404 Not Found
//    public static Result FailNotFound(this Result _, string errorMessage)
//    {
//        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, NotFoundError));
//    }

//    public static Result<T> FailNotFound<T>(this Result _, string errorMessage)
//    {
//        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, NotFoundError));
//    }

//    // 400 Bad Request
//    public static Result FailInvalid(this Result _, string errorMessage)
//    {
//        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, InvalidError));
//    }

//    public static Result<T> FailInvalid<T>(this Result _, string errorMessage)
//    {
//        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, InvalidError));
//    }

//    // 401 Unauthorized
//    public static Result FailUnauthorized(this Result _, string errorMessage)
//    {
//        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, UnauthorizedError));
//    }

//    public static Result<T> FailUnauthorized<T>(this Result _, string errorMessage)
//    {
//        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, UnauthorizedError));
//    }

//    // 403 Forbidden
//    public static Result FailForbidden(this Result _, string errorMessage)
//    {
//        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, ForbiddenError));
//    }

//    public static Result<T> FailForbidden<T>(this Result _, string errorMessage)
//    {
//        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, ForbiddenError));
//    }

//    // 409 Conflict
//    public static Result FailConflict(this Result _, string errorMessage)
//    {
//        return Result.Fail(new Error(errorMessage).WithMetadata(ErrorType, ConflictError));
//    }

//    public static Result<T> FailConflict<T>(this Result _, string errorMessage)
//    {
//        return Result.Fail<T>(new Error(errorMessage).WithMetadata(ErrorType, ConflictError));
//    }

//    // Helper methods to check error type
//    public static bool IsNotFound(this Error error) => 
//        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == NotFoundError;

//    public static bool IsInvalid(this Error error) => 
//        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == InvalidError;

//    public static bool IsUnauthorized(this Error error) => 
//        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == UnauthorizedError;

//    public static bool IsForbidden(this Error error) => 
//        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == ForbiddenError;

//    public static bool IsConflict(this Error error) => 
//        error.Metadata.ContainsKey(ErrorType) && error.Metadata[ErrorType].ToString() == ConflictError;
//}
