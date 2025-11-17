using System.Runtime.CompilerServices;

namespace Utilities.Errors;

public static class Throw
{
    public static void IfNullOrEmpty<TType>
    (
        IEnumerable<TType>? collection,
        [CallerArgumentExpression(nameof(collection))] string? paramName = null
    )
    {
        if (collection is null)
            throw new ArgumentNullException(paramName);

        if (!collection.Any())
            throw new ArgumentException("Collection must contain at least one item", paramName);
    }
}
