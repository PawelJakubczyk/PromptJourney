using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class ExampleLinkMapping
{
    public class ExampleLinkConverter : ValueConverter<ExampleLink, string>
    {
        public ExampleLinkConverter()
        : base
        (
            el => el.Value,                             // save to database
            value => ExampleLink.Create(value).Value    // read from database
        )
        { }
    }

    public class ExampleLinkComparer : ValueComparer<ExampleLink>
    {
        public ExampleLinkComparer()
        : base(
            (el1, el2) => el1!.Value == el2!.Value,     // equality
            el => el.Value.GetHashCode(),               // hashcode
            el => ExampleLink.Create(el.Value).Value    // snapshot (deep copy)
        )
        { }
    }
}