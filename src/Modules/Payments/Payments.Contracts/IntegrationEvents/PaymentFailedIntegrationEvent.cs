using BuildingBlocks.Contracts;

namespace Payments.Contracts.IntegrationEvents;

public sealed record PaymentFailedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    Guid PaymentId,
    string Reason,
    DateTime OccurredOnUtc) : IIntegrationEvent;
