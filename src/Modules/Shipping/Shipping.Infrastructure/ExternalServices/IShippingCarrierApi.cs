using Refit;

namespace Shipping.Infrastructure.ExternalServices;

public sealed record CarrierCreateLabelRequest(string Street, string City, string State, string PostalCode, string Country);
public sealed record CarrierCreateLabelResponse(bool Success, string? TrackingNumber, string? ErrorMessage);
public sealed record CarrierTrackingResponse(string Status, DateTimeOffset? EstimatedDelivery);

[Headers("Content-Type: application/json")]
public interface IShippingCarrierApi
{
    [Post("/v1/shipments/labels")]
    Task<ApiResponse<CarrierCreateLabelResponse>> CreateLabelAsync([Body] CarrierCreateLabelRequest request, CancellationToken cancellationToken = default);

    [Get("/v1/shipments/{trackingNumber}/tracking")]
    Task<ApiResponse<CarrierTrackingResponse>> GetTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default);
}
