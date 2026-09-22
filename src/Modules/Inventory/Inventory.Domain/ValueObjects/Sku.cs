using BuildingBlocks.Domain.Abstractions;

namespace Inventory.Domain.ValueObjects;

public sealed class Sku : ValueObject
{
    public string Value { get; }

    public Sku(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("SKU cannot be empty.", nameof(value));
        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(Sku sku) => sku.Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
