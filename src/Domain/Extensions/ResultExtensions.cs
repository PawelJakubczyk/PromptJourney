using FluentResults;

namespace Domain.Extensions;
public static class ResultExtensions
{
    public static List<TValue>? ToValueList<TValue>(this IEnumerable<Result<TValue>>? results)
    {

        if (results is null || !results.Any())
            return null;

        return [.. results
            .Where(result => result.IsSuccess && result.Value is not null)
            .Select(result => result.Value)];
    }
}
