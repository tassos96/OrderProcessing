using Refit;

namespace Orders.Infrastructure.ExternalServices;

public sealed record CreateShipmentRequest(Guid OrderId, string Street, string City, string State, string PostalCode, string Country);
public sealed record CreateShipmentResponse(Guid ShipmentId, string? TrackingNumber);

[Headers("Content-Type: application/json")]
public interface IShippingApi
{
    [Post("/api/shipping/create")]
    Task<ApiResponse<CreateShipmentResponse>> CreateAsync([Body] CreateShipmentRequest request, CancellationToken cancellationToken = default);
}
