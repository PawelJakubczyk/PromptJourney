using Domain.Abstractions;
using FluentResults;

namespace Application.Extension;

public static class CollectErrorsExtensions
{
    public static async Task<List<Error>> CollectErrors<TValue>
    (
        this Task<List<Error>> errorsTask,
        Result<TValue>? result,
        bool breakIfError = false)
    {
        var errors = await errorsTask;

        if (breakIfError && errors.Count != 0)
            return errors;

        if (result is not null && result.IsFailed)
        {
            errors.AddRange(result.Errors.OfType<Error>());
        }

        return errors;
    }
}
