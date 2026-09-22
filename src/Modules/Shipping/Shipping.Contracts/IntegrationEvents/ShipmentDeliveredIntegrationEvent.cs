using BuildingBlocks.Contracts;

namespace Shipping.Contracts.IntegrationEvents;

public sealed record ShipmentDeliveredIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    Guid ShipmentId,
    string TrackingNumber,
    DateTime OccurredOnUtc) : IIntegrationEvent;
