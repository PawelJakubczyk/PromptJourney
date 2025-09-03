using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class PropertyNameMapping
{
    public class PropertyNameConverter : ValueConverter<PropertyName, string>
    {
        public PropertyNameConverter()
        : base
        (
            pn => pn.Value,                             // save to database
            value => PropertyName.Create(value).Value   // read from database
        )
        { }
    }

    public class PropertyNameComparer : ValueComparer<PropertyName>
    {
        public PropertyNameComparer()
        : base(
            (pn1, pn2) => pn1!.Value == pn2!.Value,     // equality
            pn => pn.Value.GetHashCode(),               // hashcode
            pn => PropertyName.Create(pn.Value).Value   // snapshot (deep copy)
        )
        { }
    }
}