using Orders.Application.Abstractions;
using Orders.Application.DTOs;

namespace Orders.Infrastructure.ExternalServices;

public sealed class ShippingServiceClient(IShippingApi shippingApi) : IShippingService
{
    public Task<ShipmentResult> CreateShipmentAsync(Guid orderId, AddressDto address, CancellationToken cancellationToken = default)
    {
        // TODO: Call shippingApi.CreateAsync(), map response, handle errors
        // TODO: Configure retry/timeout/circuit-breaker policies according to provider SLA
        throw new NotImplementedException();
    }
}
