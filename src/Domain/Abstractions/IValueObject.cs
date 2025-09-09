using FluentResults;

namespace Domain.Abstractions;

public interface IValueObject<TSelf, TType>
    where TSelf : IValueObject<TSelf, TType>
{
    static abstract Result<TSelf> Create(TType value);
    TType Value { get; }
}