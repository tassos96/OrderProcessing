using BuildingBlocks.Domain.Abstractions;

namespace Orders.Domain.Events;

public sealed record OrderConfirmedDomainEvent(
    Guid OrderId,
    DateTime OccurredOnUtc) : IDomainEvent;
