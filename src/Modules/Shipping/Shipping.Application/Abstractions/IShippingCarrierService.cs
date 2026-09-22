namespace Shipping.Application.Abstractions;

public interface IShippingCarrierService
{
    Task<CarrierLabelResult> CreateLabelAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default);
    Task<CarrierTrackingResult> GetTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default);
}

public sealed record CarrierLabelResult(bool Success, string? TrackingNumber, string? ErrorMessage);
public sealed record CarrierTrackingResult(string Status, DateTimeOffset? EstimatedDelivery);
