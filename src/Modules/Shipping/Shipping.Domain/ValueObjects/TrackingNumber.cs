using BuildingBlocks.Domain.Abstractions;

namespace Shipping.Domain.ValueObjects;

public sealed class TrackingNumber : ValueObject
{
    public string Value { get; }

    public TrackingNumber(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Tracking number cannot be empty.", nameof(value));
        Value = value.Trim().ToUpperInvariant();
    }

    public static implicit operator string(TrackingNumber t) => t.Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
