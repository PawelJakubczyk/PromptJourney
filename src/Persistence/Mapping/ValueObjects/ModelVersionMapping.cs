using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.ConventersComparers.ValueObjects;

public sealed class ModelVersionMapping
{
    public class ModelVersionConverter : ValueConverter<ModelVersion, string>
    {
        public ModelVersionConverter()
        : base
        (
            mv => mv.Value,                             // save to database
            value => ModelVersion.Create(value).Value   // read from database
        )
        { }
    }

    public class ModelVersionComparer : ValueComparer<ModelVersion>
    {
        public ModelVersionComparer()
        : base(
            (mv1, mv2) => mv1!.Value == mv2!.Value,     // equality
            mv => mv.Value.GetHashCode(),               // hashcode
            mv => ModelVersion.Create(mv.Value).Value   // snapshot (deep copy)
        )
        { }
    }
}