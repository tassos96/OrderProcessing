using BuildingBlocks.Domain.Abstractions;

namespace Orders.Domain.Events;

public sealed record OrderCancelledDomainEvent(
    Guid OrderId,
    string Reason,
    DateTime OccurredOnUtc) : IDomainEvent;
