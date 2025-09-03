using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class TagMapping
{
    public class TagConverter : ValueConverter<Tag, string>
    {
        public TagConverter()
        : base
        (
            t => t.Value,                               // save to database
            value => Tag.Create(value).Value            // read from database
        )
        { }
    }

    public class TagComparer : ValueComparer<Tag>
    {
        public TagComparer()
        : base(
            (t1, t2) => t1!.Value == t2!.Value,         // equality
            t => t.Value.GetHashCode(),                 // hashcode
            t => Tag.Create(t.Value).Value              // snapshot (deep copy)
        )
        { }
    }

    public class TagListConverter : ValueConverter<List<Tag>?, string[]?>
    {
        public TagListConverter()
        : base
        (
            tags => tags != null ? tags.Select(t => t.Value).ToArray() : null,  // save to database
            values => values != null ? values.Select(v => Tag.Create(v).Value).ToList() : null  // read from database
        )
        { }
    }

    public class TagListComparer : ValueComparer<List<Tag>?>
    {
        public TagListComparer()
        : base(
            (t1, t2) => (t1 == null && t2 == null) || 
                       (t1 != null && t2 != null && t1.Select(x => x.Value).SequenceEqual(t2.Select(x => x.Value))),  // equality
            t => t != null ? t.Aggregate(0, (acc, tag) => acc ^ tag.Value.GetHashCode()) : 0,  // hashcode without null propagation
            t => t != null ? t.Select(tag => Tag.Create(tag.Value).Value).ToList() : null  // snapshot without null propagation
        )
        { }
    }
}