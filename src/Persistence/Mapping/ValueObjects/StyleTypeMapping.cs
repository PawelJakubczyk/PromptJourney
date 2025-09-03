using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class StyleTypeMapping
{
    public class StyleTypeConverter : ValueConverter<StyleType, string>
    {
        public StyleTypeConverter()
        : base
        (
            st => st.Value,                             // save to database
            value => StyleType.Create(value).Value      // read from database
        )
        { }
    }

    public class StyleTypeComparer : ValueComparer<StyleType>
    {
        public StyleTypeComparer()
        : base(
            (st1, st2) => st1!.Value == st2!.Value,     // equality
            st => st.Value.GetHashCode(),               // hashcode
            st => StyleType.Create(st.Value).Value      // snapshot (deep copy)
        )
        { }
    }
}