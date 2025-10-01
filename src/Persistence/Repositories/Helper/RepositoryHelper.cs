using FluentResults;
using Microsoft.AspNetCore.Http;
using Utilities.Constants;
using Utilities.Extensions;

namespace Persistence.Repositories.Helper;

public static class RepositoryHelper
{
    public static async Task<Result<TType>> ExecuteAsync<TType>(Func<Task<TType>> operation, string errorMessage, int statusCode = StatusCodes.Status500InternalServerError)
    {
        try
        {
            var result = await operation();
            return Result.Ok(result);
        }
        catch (Exception ex)
        {
            var error = ErrorFactory.Create()
                .Withlayer(typeof(PersistenceLayer))
                .WithMessage($"{errorMessage}: {ex.Message}")
                .WithErrorCode(statusCode);

            return Result.Fail<TType>(error);
        }
    }
}