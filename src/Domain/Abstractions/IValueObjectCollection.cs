namespace Domain.Abstractions;

public interface IValueObjectCollection<TType>
{
    List<TType> CollectionValues { get; }
}