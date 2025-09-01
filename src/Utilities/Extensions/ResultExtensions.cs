using FluentResults;

public static class ResultExtensions
{
    public static List<IError> CollectErrors<T>(
        this List<IError> errors,
        Result<T>? result
    )
    {
        if (result is not null && result.IsFailed)
            errors.AddRange(result.Errors);

        return errors;
    }




}
