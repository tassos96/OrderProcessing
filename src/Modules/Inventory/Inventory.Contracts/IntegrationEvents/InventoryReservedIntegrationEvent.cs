using BuildingBlocks.Contracts;

namespace Inventory.Contracts.IntegrationEvents;

public sealed record InventoryReservedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    string Sku,
    int Quantity,
    DateTime OccurredOnUtc) : IIntegrationEvent;
