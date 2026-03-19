using System.Diagnostics;

namespace Domain.Abstractions;

[DebuggerDisplay("{Value}")]
public abstract record ValueObject<TType> : IValueObject<TType>
    where TType : notnull
{
    public TType Value { get; } = default!;
    public abstract bool IsNone { get; }

    protected ValueObject(TType value)
    {
        Value = value;
    }

    public override string ToString() =>
        IsNone ? string.Empty : Value.ToString() ?? string.Empty;
    public override int GetHashCode() =>
        IsNone ? 0 : Value.GetHashCode();
}