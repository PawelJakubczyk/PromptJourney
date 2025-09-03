using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class MaxValueMapping
{
    public class MaxValueConverter : ValueConverter<MaxValue?, string?>
    {
        public MaxValueConverter()
        : base
        (
            mv => mv != null ? mv.Value : null,         // save to database
            value => value != null ? MaxValue.Create(value).Value : null  // read from database
        )
        { }
    }

    public class MaxValueComparer : ValueComparer<MaxValue?>
    {
        public MaxValueComparer()
        : base(
            (mv1, mv2) => (mv1 == null && mv2 == null) || (mv1 != null && mv2 != null && mv1.Value == mv2.Value),  // equality
            mv => mv != null ? (mv.Value != null ? mv.Value.GetHashCode() : 0) : 0,  // hashcode without null propagation
            mv => mv != null ? MaxValue.Create(mv.Value != null ? mv.Value : string.Empty).Value : null  // snapshot without null propagation
        )
        { }
    }
}