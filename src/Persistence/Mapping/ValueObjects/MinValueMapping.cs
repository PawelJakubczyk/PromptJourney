using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class MinValueMapping
{
    public class MinValueConverter : ValueConverter<MinValue?, string?>
    {
        public MinValueConverter()
        : base
        (
            mv => mv != null ? mv.Value : null,         // save to database
            value => value != null ? MinValue.Create(value).Value : null  // read from database
        )
        { }
    }

    public class MinValueComparer : ValueComparer<MinValue?>
    {
        public MinValueComparer()
        : base(
            (mv1, mv2) => (mv1 == null && mv2 == null) || (mv1 != null && mv2 != null && mv1.Value == mv2.Value),  // equality
            mv => mv != null ? (mv.Value != null ? mv.Value.GetHashCode() : 0) : 0,  // hashcode without null propagation
            mv => mv != null ? MinValue.Create(mv.Value != null ? mv.Value : string.Empty).Value : null  // snapshot without null propagation
        )
        { }
    }
}