using BuildingBlocks.Contracts;

namespace Orders.Contracts.IntegrationEvents;

public sealed record OrderCreatedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTime OccurredOnUtc) : IIntegrationEvent;
