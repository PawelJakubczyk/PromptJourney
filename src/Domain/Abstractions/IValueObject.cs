namespace Domain.Abstractions;

public interface IValueObject<TType>
{
    TType Value { get; }
}