using BuildingBlocks.Contracts;

namespace Pricing.Contracts.IntegrationEvents;

public sealed record PriceCalculatedIntegrationEvent(
    Guid EventId,
    Guid OrderId,
    decimal TotalAmount,
    DateTime OccurredOnUtc) : IIntegrationEvent;
