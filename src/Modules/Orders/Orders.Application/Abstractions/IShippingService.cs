using Orders.Application.DTOs;

namespace Orders.Application.Abstractions;

public interface IShippingService
{
    Task<ShipmentResult> CreateShipmentAsync(Guid orderId, AddressDto address, CancellationToken cancellationToken = default);
}

public sealed record ShipmentResult(Guid ShipmentId, string? TrackingNumber);
