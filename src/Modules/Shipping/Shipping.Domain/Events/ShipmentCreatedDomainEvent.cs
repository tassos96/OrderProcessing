using BuildingBlocks.Domain.Abstractions;

namespace Shipping.Domain.Events;

public sealed record ShipmentCreatedDomainEvent(
    Guid ShipmentId,
    Guid OrderId,
    DateTime OccurredOnUtc) : IDomainEvent;
