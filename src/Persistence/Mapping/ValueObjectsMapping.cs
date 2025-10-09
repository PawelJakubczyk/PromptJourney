using Domain.Abstractions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Persistence.Mapping;

public static class ValueObjectsMapping
{
    public sealed class DefaultValueConverter : ValueObjectsMapping<DefaultValue, string?>.Converter
    { }

    public sealed class DefaultValueComparer : ValueObjectsMapping<DefaultValue, string?>.Comparer
    { }

    public sealed class DescriptionConverter : ValueObjectsMapping<Description, string?>.Converter
    { }

    public sealed class DescriptionComparer : ValueObjectsMapping<Description, string?>.Comparer
    { }

    public sealed class ExampleLinkConverter : ValueObjectsMapping<ExampleLink, string>.Converter
    { }

    public sealed class ExampleLinkComparer : ValueObjectsMapping<ExampleLink, string>.Comparer
    { }

    public sealed class KeywordConverter : ValueObjectsMapping<Keyword, string>.Converter
    { }

    public sealed class KeywordComparer : ValueObjectsMapping<Keyword, string>.Comparer
    { }

    public sealed class MaxValueConverter : ValueObjectsMapping<MaxValue, string?>.Converter
    { }

    public sealed class MaxValueComparer : ValueObjectsMapping<MaxValue, string?>.Comparer
    { }

    public sealed class MinValueConverter : ValueObjectsMapping<MinValue, string?>.Converter
    { }

    public sealed class MinValueComparer : ValueObjectsMapping<MinValue, string?>.Comparer
    { }

    public sealed class ModelVersionConverter : ValueObjectsMapping<ModelVersion, string>.Converter
    { }

    public sealed class ModelVersionComparer : ValueObjectsMapping<ModelVersion, string>.Comparer
    { }

    public sealed class ParamConverter : ValueObjectsMapping<Param, string>.Converter
    { }

    public sealed class ParamComparer : ValueObjectsMapping<Param, string>.Comparer
    { }

    public sealed class ParamListConverter : ValueObjectsMapping<Param, string>.ListConverter
    { }

    public sealed class ParamListComparer : ValueObjectsMapping<Param, string>.ListComparer
    { }

    public sealed class PromptConverter : ValueObjectsMapping<Prompt, string>.Converter
    { }

    public sealed class PromptComparer : ValueObjectsMapping<Prompt, string>.Comparer
    { }

    public sealed class PropertyNameConverter : ValueObjectsMapping<PropertyName, string>.Converter
    { }

    public sealed class PropertyNameComparer : ValueObjectsMapping<PropertyName, string>.Comparer
    { }

    public sealed class StyleNameConverter : ValueObjectsMapping<StyleName, string>.Converter
    { }

    public sealed class StyleNameComparer : ValueObjectsMapping<StyleName, string>.Comparer
    { }

    public sealed class StyleTypeConverter : ValueObjectsMapping<StyleType, string>.Converter
    { }

    public sealed class StyleTypeComparer : ValueObjectsMapping<StyleType, string>.Comparer
    { }

    public sealed class TagConverter : ValueObjectsMapping<Tag, string>.Converter
    { }

    public sealed class TagComparer : ValueObjectsMapping<Tag, string>.Comparer
    { }

    public sealed class TagListConverter : ValueObjectsMapping<Tag, string>.ListConverter
    { }

    public sealed class TagListComparer : ValueObjectsMapping<Tag, string>.ListComparer
    { }
}

public static class ValueObjectsMapping<TValueObject, TValue>
    where TValueObject : class, IValueObject<TValue>, ICreatable<TValueObject, TValue>
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
            : base(
                // To database: extract primitive value
                vo => vo == null ? default : vo.Value,

                // From database: use factory method to avoid expression tree limitations
                value => value == null ? null : Factory(value)
            )
        { }
    }

    public class Comparer : ValueComparer<TValueObject?>
    {
        public Comparer()
            : base(
                // Equality comparison
                (a, b) => (a == null && b == null) ||
                          (a != null && b != null && EqualityComparer<TValue>.Default.Equals(a.Value, b.Value)),

                // GetHashCode
                vo => vo == null ? 0 : EqualityComparer<TValue>.Default.GetHashCode(vo.Value!),

                // Snapshot (deep copy)
                vo => vo == null ? null : Factory(vo.Value)
            )
        { }
    }

    public class ListConverter : ValueConverter<List<TValueObject?>?, TValue[]?>
    {
        public ListConverter()
            : base(
                // To database: convert List<TValueObject> to array of primitive values
                list => list == null ? null : list.Select(vo => vo!.Value).ToArray(),

                // From database: convert array of primitive values to List<TValueObject>
                values => values == null
                    ? null
                    : values
                        .Select(v => v != null ? Factory(v) : null)
                        .Where(vo => vo != null)
                        .ToList()
            )
        { }
    }

    public class ListComparer : ValueComparer<List<TValueObject?>?>
    {
        public ListComparer()
            : base(
                // Equality comparison - compare lists by their values using SequenceEqual
                (a, b) => (a == null && b == null) ||
                          (a != null && b != null &&
                           a.Count == b.Count &&
                           a.Select(x => x!.Value).SequenceEqual(b.Select(y => y!.Value))),

                // GetHashCode - combine hash codes of all elements
                list => list == null ? 0 : list
                    .Select(vo => vo == null ? 0 : (vo.Value == null ? 0 : EqualityComparer<TValue>.Default.GetHashCode(vo.Value)))
                    .Aggregate(17, (hash, next) => hash * 31 + next),

                // Snapshot (deep copy) - create new list with copied elements
                list => list == null ? null :
                    list.Select(vo => vo == null ? null : Factory(vo.Value))
                        .Where(vo => vo != null)
                        .ToList()
            )
        { }
    }
}