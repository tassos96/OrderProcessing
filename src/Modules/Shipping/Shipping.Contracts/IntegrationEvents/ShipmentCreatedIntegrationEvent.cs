using BuildingBlocks.Contracts;

namespace Shipping.Contracts.IntegrationEvents;

public sealed record ShipmentCreatedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    Guid ShipmentId,
    DateTime OccurredOnUtc) : IIntegrationEvent;
