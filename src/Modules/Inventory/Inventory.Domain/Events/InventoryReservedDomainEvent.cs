using BuildingBlocks.Domain.Abstractions;

namespace Inventory.Domain.Events;

public sealed record InventoryReservedDomainEvent(
    Guid InventoryItemId,
    int Quantity,
    Guid OrderId,
    DateTime OccurredOnUtc) : IDomainEvent;
