using BuildingBlocks.Contracts;

namespace Orders.Contracts.IntegrationEvents;

public sealed record OrderConfirmedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    DateTime OccurredOnUtc) : IIntegrationEvent;
