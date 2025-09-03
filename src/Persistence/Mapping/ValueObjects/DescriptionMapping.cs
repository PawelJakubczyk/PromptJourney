using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class DescriptionMapping
{
    public class DescriptionConverter : ValueConverter<Description?, string?>
    {
        public DescriptionConverter()
        : base
        (
            d => d != null ? d.Value : null,            // save to database
            value => value != null ? Description.Create(value).Value : null  // read from database
        )
        { }
    }

    public class DescriptionComparer : ValueComparer<Description?>
    {
        public DescriptionComparer()
        : base(
            (d1, d2) => (d1 == null && d2 == null) || (d1 != null && d2 != null && d1.Value == d2.Value),  // equality
            d => d != null ? (d.Value != null ? d.Value.GetHashCode() : 0) : 0,  // hashcode without null propagation
            d => d != null ? Description.Create(d.Value != null ? d.Value : string.Empty).Value : null  // snapshot without null propagation
        )
        { }
    }
}