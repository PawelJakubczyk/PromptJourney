using Domain.Abstractions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping;

public static class ValueObjectsMapping
{
    // ================================================================
    // SIMPLE VALUE OBJECT CONVERTERS & COMPARERS
    // ================================================================

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

    // ================================================================
    // COLLECTION VALUE OBJECT CONVERTERS & COMPARERS
    // ================================================================

    public sealed class ParamsCollectionConverter : CollectionValueObjectMapping<ParamsCollection, Param, string>.Converter { }
    public sealed class ParamsCollectionComparer : CollectionValueObjectMapping<ParamsCollection, Param, string>.Comparer { }

    public sealed class TagsCollectionConverter : CollectionValueObjectMapping<TagsCollection, Tag, string>.Converter { }
    public sealed class TagsCollectionComparer : CollectionValueObjectMapping<TagsCollection, Tag, string>.Comparer { }

    // ================================================================
    // DATE VALUE OBJECT (DateTimeOffset) CONVERTERS & COMPARERS
    // ================================================================

    public sealed class ReleaseDateConverter : DateTimeValueObjectMapping<ReleaseDate>.Converter { }

    public sealed class ReleaseDateComparer : DateTimeValueObjectMapping<ReleaseDate>.Comparer { }


    public sealed class CreatedOnConverter : DateTimeValueObjectMapping<CreatedOn>.Converter { }

    public sealed class CreatedOnComparer : DateTimeValueObjectMapping<CreatedOn>.Comparer { }

    // ================================================================
    // ID VALUE OBJECT (Guid) CONVERTERS & COMPARERS
    // ================================================================

    public sealed class LinkIDConverter : GuidValueObjectMapping<LinkID>.Converter { }

    public sealed class LinkIDComparer : GuidValueObjectMapping<LinkID>.Comparer { }


    public sealed class HistoryIDConverter : GuidValueObjectMapping<HistoryID>.Converter { }

    public sealed class HistoryIDComparer : GuidValueObjectMapping<HistoryID>.Comparer { }
}

// ================================================================
// GENERIC MAPPING FOR SIMPLE VALUE OBJECTS
// ================================================================

public static class ValueObjectsMapping<TValueObject, TValue>
    where TValueObject : notnull, ValueObject<TValue>, ICreatable<TValueObject, TValue?>
    where TValue : notnull
{
    // Factory delegate to create objects outside of expression trees
    private static readonly Func<TValue?, TValueObject> Factory = value =>
    {
        var result = TValueObject.Create(value);
        return result.IsSuccess ? result.Value : throw new InvalidOperationException("Failed to create value object.");
    };

    public class Converter : ValueConverter<TValueObject, TValue?>
    {
        public Converter()
            : base
            (
                // To database: extract primitive value, handle None pattern
                vo => vo.IsNone || vo == null ? default : vo.Value,

                // From database: use factory method to avoid expression tree limitations
                value => Factory(value)
            )
        { }
    }

    public class Comparer : ValueComparer<TValueObject>
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
                vo => vo == null || vo.IsNone ? Factory(default) : Factory(vo.Value)
            )
        { }
    }
}

// ================================================================
// GENERIC MAPPING FOR COLLECTION VALUE OBJECTS
// ================================================================

public static class CollectionValueObjectMapping<TCollectionValueObject, TItemValueObject, TItemValue>
    where TCollectionValueObject : notnull, ValueObject<List<TItemValueObject>>, ICreatable<TCollectionValueObject, List<TItemValue?>?>
    where TItemValueObject : notnull, ValueObject<TItemValue>, ICreatable<TItemValueObject, TItemValue?>
    where TItemValue : notnull
    {
        // Factory for collection - creates collection from list of primitive values
        private static readonly Func<TItemValue[]?, TCollectionValueObject> CollectionFactory = values =>
        {
            var result = TCollectionValueObject.Create([.. values ?? []]);
            return result.IsSuccess ? result.Value : throw new InvalidOperationException("Failed to create value object.");
        };

        public class Converter : ValueConverter<TCollectionValueObject, TItemValue[]?>
        {
            public Converter()
                : base
                (
                    // To database: convert ValueObject<List<ItemValueObject>> to TItemValue[]
                    collection => collection.IsNone
                        ? null
                        : collection.Value.Select(item => item.Value).ToArray(),

                    // From database: convert TItemValue[] to ValueObject<List<ItemValueObject>>
                    values => CollectionFactory(values)
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

// ================================================================
// GENERIC MAPPING FOR DATE TIME VALUE OBJECTS
// ================================================================

public static class DateTimeValueObjectMapping<TValueObject>
    where TValueObject : notnull, ValueObject<DateTimeOffset>, ICreatable<TValueObject, string?>
    {
        private static readonly Func<string?, TValueObject> Factory = value =>
        {
            var result = TValueObject.Create(value);
            return result.IsSuccess ? result.Value  : throw new InvalidOperationException( $"Failed to create {typeof(TValueObject).Name} from '{value}'.");
        };

        // To database: convert ValueObject<List<ItemValueObject>> to DataTimeOffset
        public class Converter : ValueConverter<TValueObject, DateTimeOffset>
        {
            public Converter()
                : base
                (
                    // Domain -> DB
                    vo => vo.Value,

                    // DB -> Domain
                    dto => Factory(dto.ToString("O"))
                )
            { }
        }

        // From database: convert DateTimeOffset to ValueObject<List<ItemValueObject>>
        public class Comparer : ValueComparer<TValueObject>
        {
            public Comparer()
                : base(
                    // Equality
                    (a, b) => a != null && b != null && a.Value.Equals(b.Value),

                    // HashCode
                    vo => vo.Value.GetHashCode(),

                    // Snapshot
                    vo => vo == null || vo.IsNone ? Factory(default) : Factory(vo.Value.ToString("O"))
                )
            { }
        }
    }


public static class GuidValueObjectMapping<TValueObject>
    where TValueObject : ValueObject<Guid>, ICreatable<TValueObject, string?>
{
    private static readonly Func<string, TValueObject> Factory = value =>
    {
        var result = TValueObject.Create(value);
        return result.IsSuccess
            ? result.Value
            : throw new InvalidOperationException(
                $"Failed to create {typeof(TValueObject).Name} from '{value}'.");
    };

    public class Converter : ValueConverter<TValueObject, Guid>
    {
        public Converter()
            : base(
                // Domain -> DB
                vo => vo.Value,

                // DB -> Domain
                guid => Factory(guid.ToString("N"))
            )
        { }
    }

    public class Comparer : ValueComparer<TValueObject>
    {
        public Comparer()
            : base(
                // Equality
                (a, b) => a.Value.Equals(b.Value),

                // HashCode
                vo => vo.Value.GetHashCode(),

                // Snapshot (deep copy)
                vo => Factory(vo.Value.ToString("N"))
            )
        { }
    }
}