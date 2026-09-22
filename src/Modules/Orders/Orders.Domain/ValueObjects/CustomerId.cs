using BuildingBlocks.Domain.Abstractions;

namespace Orders.Domain.ValueObjects;

public sealed class CustomerId : ValueObject
{
    public Guid Value { get; }

    public CustomerId(Guid value) => Value = value == Guid.Empty
        ? throw new ArgumentException("CustomerId cannot be empty.", nameof(value))
        : value;

    public static implicit operator Guid(CustomerId id) => id.Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
