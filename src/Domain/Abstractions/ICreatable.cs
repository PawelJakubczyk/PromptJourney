using FluentResults;

namespace Domain.Abstractions;

public interface ICreatable<TSelf, TType>
    where TSelf : ICreatable<TSelf, TType>
{
    static abstract Result<TSelf> Create(TType value);
}
