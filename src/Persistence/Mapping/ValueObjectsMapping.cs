using Domain.Abstractions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping;

public static class ValueObjectsMapping
{
    // ======================================================
    // SIMPLE VALUE OBJECT CONVERTERS & COMPARERS
    // ======================================================

    public sealed class DefaultValueConverter : ValueObjectsMapping<DefaultValue, string>.Converter { }
    public sealed class DefaultValueComparer : ValueObjectsMapping<DefaultValue, string>.Comparer { }

    public sealed class DescriptionConverter : ValueObjectsMapping<Description, string>.Converter { }
    public sealed class DescriptionComparer : ValueObjectsMapping<Description, string>.Comparer { }

    public sealed class LinkConverter : ValueObjectsMapping<ExampleLink, string>.Converter { }
    public sealed class LinkComparer : ValueObjectsMapping<ExampleLink, string>.Comparer { }

    public sealed class KeywordConverter : ValueObjectsMapping<Keyword, string>.Converter { }
    public sealed class KeywordComparer : ValueObjectsMapping<Keyword, string>.Comparer { }

    public sealed class MaxValueConverter : ValueObjectsMapping<MaxValue, string>.Converter { }
    public sealed class MaxValueComparer : ValueObjectsMapping<MaxValue, string>.Comparer { }

    public sealed class MinValueConverter : ValueObjectsMapping<MinValue, string>.Converter { }
    public sealed class MinValueComparer : ValueObjectsMapping<MinValue, string>.Comparer { }

    public sealed class ModelVersionConverter : ValueObjectsMapping<ModelVersion, string>.Converter { }
    public sealed class ModelVersionComparer : ValueObjectsMapping<ModelVersion, string>.Comparer { }

    public sealed class ParamConverter : ValueObjectsMapping<Param, string>.Converter { }
    public sealed class ParamComparer : ValueObjectsMapping<Param, string>.Comparer { }

    public sealed class PromptConverter : ValueObjectsMapping<Prompt, string>.Converter { }
    public sealed class PromptComparer : ValueObjectsMapping<Prompt, string>.Comparer { }

    public sealed class PropertyNameConverter : ValueObjectsMapping<PropertyName, string>.Converter { }
    public sealed class PropertyNameComparer : ValueObjectsMapping<PropertyName, string>.Comparer { }

    public sealed class StyleNameConverter : ValueObjectsMapping<StyleName, string>.Converter { }
    public sealed class StyleNameComparer : ValueObjectsMapping<StyleName, string>.Comparer { }

    public sealed class StyleTypeConverter : ValueObjectsMapping<StyleType, string>.Converter { }
    public sealed class StyleTypeComparer : ValueObjectsMapping<StyleType, string>.Comparer { }

    public sealed class TagConverter : ValueObjectsMapping<Tag, string>.Converter { }
    public sealed class TagComparer : ValueObjectsMapping<Tag, string>.Comparer { }

    // ======================================================
    // COLLECTION VALUE OBJECT CONVERTERS & COMPARERS
    // ======================================================

    public sealed class ParamsCollectionConverter : CollectionValueObjectMapping<ParamsCollection, Param, string>.Converter { }
    public sealed class ParamsCollectionComparer : CollectionValueObjectMapping<ParamsCollection, Param, string>.Comparer { }

    public sealed class TagsCollectionConverter : CollectionValueObjectMapping<TagsCollection, Tag, string>.Converter { }
    public sealed class TagsCollectionComparer : CollectionValueObjectMapping<TagsCollection, Tag, string>.Comparer { }

    // ======================================================
    // SPECIAL CONVERTER FOR RELEASEDATE (DateTimeOffset)
    // ======================================================

    public sealed class ReleaseDateConverter
        : ValueConverter<ReleaseDate?, DateTimeOffset?>
    {
        public ReleaseDateConverter()
            : base(
                // Domain -> DB
                releaseDate => releaseDate == null ? null : releaseDate.Value,

                // DB -> Domain
                value => value == null ? null : ReleaseDate.Create(value.Value.ToUniversalTime().ToString("O")).Value
            )
        { }
    }

    public sealed class ReleaseDateComparer
        : ValueComparer<ReleaseDate?>
    {
        public ReleaseDateComparer()
            : base(
                // Equality comparison
                (a, b) => (a == null && b == null) ||
                          (a != null && b != null && a.Value.Equals(b.Value)),

                // GetHashCode
                releaseDate => releaseDate == null ? 0 : releaseDate.Value.GetHashCode(),

                // Snapshot (deep copy)
                releaseDate => releaseDate == null ? null : ReleaseDate.Create(releaseDate.Value.ToString("O")).Value
            )
        { }
    }
}

// ======================================================
// GENERIC MAPPING FOR SIMPLE VALUE OBJECTS
// ======================================================

public static class ValueObjectsMapping<TValueObject, TValue>
    where TValueObject : ValueObject<TValue>, ICreatable<TValueObject, TValue>
    where TValue : notnull
{
    // Factory delegate to create objects outside of expression trees
    private static readonly Func<TValue, TValueObject?> Factory = value =>
    {
        var result = TValueObject.Create(value);
        return result.IsSuccess ? result.Value : null;
    };

    public class Converter : ValueConverter<TValueObject?, TValue?>
    {
        public Converter()
            : base
            (
                // To database: extract primitive value, handle None pattern
                vo => vo == null || vo.IsNone ? default : vo.Value,

                // From database: use factory method to avoid expression tree limitations
                value => value == null || EqualityComparer<TValue>.Default.Equals(value, default) 
                    ? null 
                    : Factory(value)
            )
        { }
    }

    public class Comparer : ValueComparer<TValueObject?>
    {
        public Comparer()
            : base
            (
                // Equality comparison - handle None and null cases
                (a, b) => (a == null && b == null) ||
                          (a != null && b != null && 
                           ((a.IsNone && b.IsNone) || EqualityComparer<TValue>.Default.Equals(a.Value, b.Value))),

                // GetHashCode - handle None and null
                vo => vo == null || vo.IsNone ? 0 : EqualityComparer<TValue>.Default.GetHashCode(vo.Value),

                // Snapshot (deep copy)
                vo => vo == null || vo.IsNone ? null : Factory(vo.Value)
            )
        { }
    }
}

// ======================================================
// GENERIC MAPPING FOR COLLECTION VALUE OBJECTS
// ======================================================

public static class CollectionValueObjectMapping<TCollectionValueObject, TItemValueObject, TItemValue>
    where TCollectionValueObject : ValueObject<List<TItemValueObject>>, ICreatable<TCollectionValueObject, List<TItemValue>>
    where TItemValueObject : ValueObject<TItemValue>, ICreatable<TItemValueObject, TItemValue>
    where TItemValue : notnull
{
    // Factory for collection - creates collection from list of primitive values
    private static readonly Func<TItemValue[], TCollectionValueObject?> CollectionFactory = values =>
    {
        var result = TCollectionValueObject.Create(new List<TItemValue>(values));
        return result.IsSuccess ? result.Value : null;
    };

    public class Converter : ValueConverter<TCollectionValueObject?, TItemValue[]?>
    {
        public Converter()
            : base
            (
                // To database: convert ValueObject<List<ItemValueObject>> to TItemValue[]
                collection => collection == null || collection.IsNone
                    ? null
                    : collection.Value.Select(item => item.Value).ToArray(),

                // From database: convert TItemValue[] to ValueObject<List<ItemValueObject>>
                values => values == null || values.Length == 0
                    ? null
                    : CollectionFactory(values)
            )
        { }
    }

    public class Comparer : ValueComparer<TCollectionValueObject?>
    {
        public Comparer()
            : base
            (
                // Equality comparison - compare collections by their values using SequenceEqual
                (a, b) => (a == null && b == null) ||
                          (a != null && b != null &&
                           ((a.IsNone && b.IsNone) ||
                            (a.Value.Count == b.Value.Count &&
                             a.Value.Select(x => x.Value).SequenceEqual(b.Value.Select(y => y.Value))))),

                // GetHashCode - combine hash codes of all elements
                collection => collection == null || collection.IsNone
                    ? 0
                    : collection.Value
                        .Select(item => EqualityComparer<TItemValue>.Default.GetHashCode(item.Value))
                        .Aggregate(17, (hash, next) => hash * 31 + next),

                // Snapshot (deep copy) - create new collection with copied elements
                collection => collection == null || collection.IsNone
                    ? null
                    : CollectionFactory(collection.Value.Select(item => item.Value).ToArray())
            )
        { }
    }
}
