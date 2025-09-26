using FluentResults;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Constants;
using Utilities.Errors;

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
            var error = new Error<PersistenceLayer>($"{errorMessage}: {ex.Message}", statusCode);
            return Result.Fail<TType>(error);
        }
    }
}