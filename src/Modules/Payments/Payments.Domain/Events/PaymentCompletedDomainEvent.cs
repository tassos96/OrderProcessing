using BuildingBlocks.Domain.Abstractions;

namespace Payments.Domain.Events;

public sealed record PaymentCompletedDomainEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    DateTime OccurredOnUtc) : IDomainEvent;
