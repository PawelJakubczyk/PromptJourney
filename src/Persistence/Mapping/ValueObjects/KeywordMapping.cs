using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class KeywordMapping
{
    public class KeywordConverter : ValueConverter<Keyword, string>
    {
        public KeywordConverter()
        : base
        (
            k => k.Value,                               // save to database
            value => Keyword.Create(value).Value        // read from database
        )
        { }
    }

    public class KeywordComparer : ValueComparer<Keyword>
    {
        public KeywordComparer()
        : base(
            (k1, k2) => k1!.Value == k2!.Value,         // equality
            k => k.Value.GetHashCode(),                 // hashcode
            k => Keyword.Create(k.Value).Value          // snapshot (deep copy)
        )
        { }
    }
}