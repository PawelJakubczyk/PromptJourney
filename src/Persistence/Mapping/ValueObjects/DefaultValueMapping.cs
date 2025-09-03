using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class DefaultValueMapping
{
    public class DefaultValueConverter : ValueConverter<DefaultValue?, string?>
    {
        public DefaultValueConverter()
        : base
        (
            dv => dv != null ? dv.Value : null,         // save to database
            value => value != null ? DefaultValue.Create(value).Value : null  // read from database
        )
        { }
    }

    public class DefaultValueComparer : ValueComparer<DefaultValue?>
    {
        public DefaultValueComparer()
        : base(
            (dv1, dv2) => (dv1 == null && dv2 == null) || (dv1 != null && dv2 != null && dv1.Value == dv2.Value),  // equality
            dv => dv != null ? (dv.Value != null ? dv.Value.GetHashCode() : 0) : 0,  // hashcode without null propagation
            dv => dv != null ? DefaultValue.Create(dv.Value != null ? dv.Value : string.Empty).Value : null  // snapshot without null propagation
        )
        { }
    }
}