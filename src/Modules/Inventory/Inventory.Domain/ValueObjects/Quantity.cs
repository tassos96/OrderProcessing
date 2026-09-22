using BuildingBlocks.Domain.Abstractions;

namespace Inventory.Domain.ValueObjects;

public sealed class Quantity : ValueObject
{
    public int Value { get; }

    public Quantity(int value)
    {
        if (value < 0) throw new ArgumentException("Quantity cannot be negative.", nameof(value));
        Value = value;
    }

    public static implicit operator int(Quantity qty) => qty.Value;

    public Quantity Add(int amount) => new(Value + amount);

    public Quantity Subtract(int amount) => new(Value - amount);

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
