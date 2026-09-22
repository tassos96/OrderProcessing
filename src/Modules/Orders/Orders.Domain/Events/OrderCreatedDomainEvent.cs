using BuildingBlocks.Domain.Abstractions;

namespace Orders.Domain.Events;

public sealed record OrderCreatedDomainEvent(
    Guid OrderId,
    Guid CustomerId,
    DateTime OccurredOnUtc) : IDomainEvent;
