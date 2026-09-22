using BuildingBlocks.Contracts;

namespace Inventory.Contracts.IntegrationEvents;

public sealed record InventoryReleasedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    string Sku,
    int Quantity,
    DateTime OccurredOnUtc) : IIntegrationEvent;
