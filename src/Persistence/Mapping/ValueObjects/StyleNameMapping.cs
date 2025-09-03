using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class StyleNameMapping
{
    public class StyleNameConverter : ValueConverter<StyleName, string>
    {
        public StyleNameConverter()
        : base
        (
            sn => sn.Value,                             // save to database
            value => StyleName.Create(value).Value      // read from database
        )
        { }
    }

    public class StyleNameComparer : ValueComparer<StyleName>
    {
        public StyleNameComparer()
        : base(
            (sn1, sn2) => sn1!.Value == sn2!.Value,     // equality
            sn => sn.Value.GetHashCode(),               // hashcode
            sn => StyleName.Create(sn.Value).Value      // snapshot (deep copy)
        )
        { }
    }
}