using Shipping.Application.Abstractions;

namespace Shipping.Infrastructure.ExternalServices;

public sealed class ShippingCarrierClient(IShippingCarrierApi carrierApi) : IShippingCarrierService
{
    public Task<CarrierLabelResult> CreateLabelAsync(string street, string city, string state, string postalCode, string country, CancellationToken cancellationToken = default)
    {
        // TODO: Call carrierApi.CreateLabelAsync(), map response
        // TODO: Configure retry/timeout/circuit-breaker policies according to provider SLA
        throw new NotImplementedException();
    }

    public Task<CarrierTrackingResult> GetTrackingAsync(string trackingNumber, CancellationToken cancellationToken = default)
    {
        // TODO: Call carrierApi.GetTrackingAsync(), map response
        throw new NotImplementedException();
    }
}
