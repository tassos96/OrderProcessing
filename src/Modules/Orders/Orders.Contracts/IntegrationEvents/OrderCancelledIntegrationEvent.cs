using BuildingBlocks.Contracts;

namespace Orders.Contracts.IntegrationEvents;

public sealed record OrderCancelledIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    string Reason,
    DateTime OccurredOnUtc) : IIntegrationEvent;
