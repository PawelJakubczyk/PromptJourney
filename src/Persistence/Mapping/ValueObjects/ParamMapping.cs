using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping.ValueObjects;

public sealed class ParamMapping
{
    public class ParamConverter : ValueConverter<Param, string>
    {
        public ParamConverter()
        : base
        (
            p => p.Value,                               // save to database
            value => Param.Create(value).Value          // read from database
        )
        { }
    }

    public class ParamComparer : ValueComparer<Param>
    {
        public ParamComparer()
        : base(
            (p1, p2) => p1!.Value == p2!.Value,         // equality
            p => p.Value.GetHashCode(),                 // hashcode
            p => Param.Create(p.Value).Value            // snapshot (deep copy)
        )
        { }
    }

    public class ParamListConverter : ValueConverter<List<Param>?, string[]?>
    {
        public ParamListConverter()
        : base
        (
            parameters => parameters != null ? parameters.Select(p => p.Value).ToArray() : null,  // save to database
            values => values != null ? values.Select(v => Param.Create(v).Value).ToList() : null  // read from database
        )
        { }
    }

    public class ParamListComparer : ValueComparer<List<Param>?>
    {
        public ParamListComparer()
        : base(
            (p1, p2) => (p1 == null && p2 == null) || 
                       (p1 != null && p2 != null && p1.Select(x => x.Value).SequenceEqual(p2.Select(x => x.Value))),  // equality
            p => p != null ? p.Aggregate(0, (acc, param) => acc ^ param.Value.GetHashCode()) : 0,  // hashcode without null propagation
            p => p != null ? p.Select(param => Param.Create(param.Value).Value).ToList() : null  // snapshot without null propagation
        )
        { }
    }
}