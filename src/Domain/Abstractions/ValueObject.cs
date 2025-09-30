namespace Domain.Abstractions;

public abstract record ValueObject<TType> : IValueObject<TType?>
{
    public TType? Value { get; } = default;

    protected ValueObject(TType? value)
    {
        Value = value;
    }

    public sealed override string? ToString() => Value?.ToString() ?? null;

    public override int GetHashCode()
    {
        if (Value == null)
            return 0;
        return Value.GetHashCode();
    }

    public virtual bool Equals(ValueObject<TType>? obj)
    {
        if (obj is not ValueObject<TType> other)
            return false;
        if (Value == null && other.Value == null)
            return true;
        if (Value == null || other.Value == null)
            return false;
        return Value.Equals(other.Value);
    }
}