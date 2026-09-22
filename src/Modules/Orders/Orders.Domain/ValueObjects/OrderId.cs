using BuildingBlocks.Domain.Abstractions;

namespace Orders.Domain.ValueObjects;

public sealed class OrderId : ValueObject
{
    public Guid Value { get; }

    public OrderId(Guid value) => Value = value == Guid.Empty
        ? throw new ArgumentException("OrderId cannot be empty.", nameof(value))
        : value;

    public static OrderId New() => new(Guid.NewGuid());
    public static implicit operator Guid(OrderId id) => id.Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
