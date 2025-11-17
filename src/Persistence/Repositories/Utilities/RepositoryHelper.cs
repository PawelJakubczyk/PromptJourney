using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Errors;

namespace Persistence.Repositories.Utilities;

public static class RepositoryHelper
{
    public static async Task<Result<TType>> ExecuteAsync<TType>
    (
        Func<Task<TType>> operation, 
        string errorMessage, 
        int statusCode = StatusCodes.Status500InternalServerError
    )
    {
        try
        {
            var result = await operation();
            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            var error = ErrorBuilder.New()
                .WithLayer<PersistenceLayer>()
                .WithMessage($"{errorMessage}: {ex.Message}")
                .WithErrorCode(statusCode)
                .Build();

            return Result.Fail<TType>(error);
        }
    }

    public static async Task<Result<TType>> ExecuteAsync<TType>
    (
        Func<Task<Result<TType>>> operation, 
        string errorMessage, 
        int statusCode = StatusCodes.Status500InternalServerError
    )
    {
        try
        {
            var result = await operation();
            return result;
        }
        catch (Exception ex)
        {
            var error = ErrorBuilder.New()
                .WithLayer<PersistenceLayer>()
                .WithMessage($"{errorMessage}: {ex.Message}")
                .WithErrorCode(statusCode)
                .Build();

            return Result.Fail<TType>(error);
        }
    }
}
