using BuildingBlocks.Domain.Abstractions;

namespace Orders.Domain.Entities;

public sealed class OrderItem : Entity<Guid>
{
    public string Sku { get; private set; } = default!;
    public string ProductName { get; private set; } = default!;
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    private OrderItem() { }

    internal OrderItem(string sku, string productName, int quantity, decimal unitPrice)
        : base(Guid.NewGuid())
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
        if (unitPrice < 0) throw new ArgumentException("Unit price cannot be negative.", nameof(unitPrice));
        Sku = sku;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
