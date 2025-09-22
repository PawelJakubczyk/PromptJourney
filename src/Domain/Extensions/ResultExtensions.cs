using FluentResults;

namespace Domain.Extensions;
public static class ResultExtensions
{
    public static List<TValue> ToValueList<TValue>(this IEnumerable<Result<TValue>> results)
    {
        return [.. results
            .Where(r => r.IsSuccess && r.Value is not null)
            .Select(r => r.Value)];
    }
}
