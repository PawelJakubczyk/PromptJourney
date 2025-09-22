using FluentResults;

namespace Application.Extension;

public static class RepositoryActionExtensions
{
    public static async Task<Result<TResponse>> ExecuteAndMapResultIfNoErrors<TType, TResponse>
    (
        this Task<List<Error>> errorsTask,
        Func<Task<Result<TType>>> repositoryAction,
        Func<TType, TResponse> mapResponse
    )
    {
        var errors = await errorsTask;

        if (errors.Count != 0)
            return Result.Fail<TResponse>(errors);

        var result = await repositoryAction();

        if (result.IsFailed)
        {
            errors.AddRange(result.Errors.OfType<Error>());
            return Result.Fail<TResponse>(errors);
        }

        var response = mapResponse(result.Value);
        return Result.Ok(response);
    }
}
