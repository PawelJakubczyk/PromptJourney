using Domain.Abstractions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping;

public static class ValueObjects
{
    public sealed class DefaultValueMapping : ValueObjectMappingBase<DefaultValue, string> { }

    public sealed class DescriptionMapping : ValueObjectMappingBase<Description, string> { }

    public sealed class ExampleLinkMapping : ValueObjectMappingBase<ExampleLink, string> { }

    public sealed class KeywordMapping : ValueObjectMappingBase<Keyword, string> { }

    public sealed class MaxValueMapping : ValueObjectMappingBase<MaxValue, string> { }

    public sealed class MinValueMapping : ValueObjectMappingBase<MinValue, string> { }

    public sealed class ModelVersionMapping : ValueObjectMappingBase<ModelVersion, string> { }

    public sealed class ParamMapping : ValueObjectMappingBase<Param, string> { }

    public sealed class ParamListMapping : ValueObjectListMappingBase<Param, string> { }

    public sealed class PromptMapping : ValueObjectMappingBase<Prompt, string> { }

    public sealed class PropertyNameMapping : ValueObjectMappingBase<PropertyName, string> { }

    public sealed class StyleNameMapping : ValueObjectMappingBase<StyleName, string> { }

    public sealed class StyleTypeMapping : ValueObjectMappingBase<StyleType, string> { }

    public sealed class TagMapping : ValueObjectMappingBase<Tag, string> { }

    public sealed class TagListMapping : ValueObjectListMappingBase<Tag, string> { }
}

// Original mapping for single value objects
public class ValueObjectMappingBase<TValueObject, TValue>
    where TValueObject : class, IValueObject<TValueObject, TValue>
{
    // Factory delegate to create objects outside of expression trees
    private static readonly Func<TValue, TValueObject?> Factory = value =>
    {
        var result = TValueObject.Create(value);
        return result.IsSuccess ? result.Value : null;
    };

    public sealed class Converter : ValueConverter<TValueObject?, TValue?>
    {
        public Converter()
            : base(
                // To database: extract primitive value
                vo => vo == null ? default : vo.Value,

                // From database: use factory method to avoid expression tree limitations
                value => value == null ? null : Factory(value)
            )
        { }
    }

    public sealed class Comparer : ValueComparer<TValueObject?>
    {
        public Comparer()
            : base(
                // Equality comparison
                (a, b) => (a == null && b == null) ||
                          (a != null && b != null && EqualityComparer<TValue>.Default.Equals(a.Value, b.Value)),

                // GetHashCode
                vo => vo == null ? 0 : EqualityComparer<TValue>.Default.GetHashCode(vo.Value),
                vo => vo == null ? null : Factory(vo.Value)
            )
        { }
    }
}

public class ValueObjectListMappingBase<TValueObject, TValue>
    where TValueObject : class, IValueObject<TValueObject, TValue>
{
    // Factory delegate to create objects outside of expression trees
    private static readonly Func<TValue, TValueObject?> Factory = value =>
    {
        var result = TValueObject.Create(value);
        return result.IsSuccess ? result.Value : null;
    };

    public sealed class Converter : ValueConverter<List<TValueObject>?, TValue[]?>
    {
        public Converter()
            : base(
                // To database: convert List<TValueObject> to TValue[]
                list => list == null ? null : list.Select(vo => vo.Value).ToArray(),

                // From database: convert TValue[] to List<TValueObject>
                values => values == null ? null : 
                    values.Select(Factory)
                          .Where(vo => vo != null)
                          .ToList()!
            )
        { }
    }

    public sealed class Comparer : ValueComparer<List<TValueObject>?>
    {
        public Comparer()
            : base(
                // Equality comparison
                (a, b) => (a == null && b == null) ||
                          (a != null && b != null && 
                           a.Count == b.Count && 
                           a.Select(vo => vo.Value).SequenceEqual(b.Select(vo => vo.Value))),

                // GetHashCode
                list => list == null ? 0 : 
                    list.Select(vo => EqualityComparer<TValue>.Default.GetHashCode(vo.Value))
                        .Aggregate(0, (acc, hash) => acc ^ hash),

                // Snapshot (deep copy)
                list => list == null ? null : 
                    list.Select(vo => Factory(vo.Value))
                        .Where(vo => vo != null)
                        .ToList()!
            )
        { }
    }
}
