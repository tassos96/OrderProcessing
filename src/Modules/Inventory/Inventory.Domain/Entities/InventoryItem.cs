using BuildingBlocks.Domain.Abstractions;
using Inventory.Domain.Events;
using Inventory.Domain.Exceptions;
using Inventory.Domain.ValueObjects;

namespace Inventory.Domain.Entities;

public sealed class InventoryItem : AggregateRoot<Guid>
{
    public Sku Sku { get; private set; } = default!;
    public Quantity AvailableQuantity { get; private set; } = default!;
    public Quantity ReservedQuantity { get; private set; } = default!;

    private InventoryItem() { }

    public static InventoryItem Create(Sku sku, int initialQuantity)
    {
        return new InventoryItem
        {
            Id = Guid.NewGuid(),
            Sku = sku,
            AvailableQuantity = new Quantity(initialQuantity),
            ReservedQuantity = new Quantity(0),
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Reserve(int quantity, Guid orderId)
    {
        if (AvailableQuantity.Value < quantity)
            throw new InventoryDomainException($"Insufficient stock for SKU '{Sku}'. Available: {AvailableQuantity.Value}, Requested: {quantity}");

        AvailableQuantity = AvailableQuantity.Subtract(quantity);
        ReservedQuantity = ReservedQuantity.Add(quantity);
        AddDomainEvent(new InventoryReservedDomainEvent(Id, quantity, orderId, DateTime.UtcNow));
    }

    public void Release(int quantity, Guid orderId)
    {
        if (ReservedQuantity.Value < quantity)
            throw new InventoryDomainException($"Cannot release more than reserved for SKU '{Sku}'.");

        ReservedQuantity = ReservedQuantity.Subtract(quantity);
        AvailableQuantity = AvailableQuantity.Add(quantity);
        AddDomainEvent(new InventoryReleasedDomainEvent(Id, quantity, orderId, DateTime.UtcNow));
    }

    public void Deduct(int quantity)
    {
        // TODO: Implement deduction from reserved stock after order completion
        throw new NotImplementedException();
    }
}
