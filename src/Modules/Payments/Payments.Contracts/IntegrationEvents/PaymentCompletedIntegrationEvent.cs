using BuildingBlocks.Contracts;

namespace Payments.Contracts.IntegrationEvents;

public sealed record PaymentCompletedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    DateTime OccurredOnUtc) : IIntegrationEvent;
