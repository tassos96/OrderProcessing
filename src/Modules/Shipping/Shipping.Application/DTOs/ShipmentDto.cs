namespace Shipping.Application.DTOs;

public sealed record ShipmentDto(
    Guid Id,
    Guid OrderId,
    string Status,
    string? TrackingNumber,
    DateTimeOffset? EstimatedDelivery);
