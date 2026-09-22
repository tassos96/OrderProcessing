using BuildingBlocks.Domain.Abstractions;

namespace Payments.Domain.Events;

public sealed record PaymentFailedDomainEvent(
    Guid PaymentId,
    Guid OrderId,
    string Reason,
    DateTime OccurredOnUtc) : IDomainEvent;
