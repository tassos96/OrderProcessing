using Orders.Application.Abstractions;

namespace Orders.Infrastructure.ExternalServices;

public sealed class InventoryServiceClient(IInventoryApi inventoryApi) : IInventoryService
{
    public Task<bool> ReserveStockAsync(Guid orderId, string sku, int quantity, CancellationToken cancellationToken = default)
    {
        // TODO: Call inventoryApi.ReserveAsync(), map response, handle errors
        // TODO: Configure retry/timeout/circuit-breaker policies according to provider SLA
        throw new NotImplementedException();
    }

    public Task ReleaseStockAsync(Guid orderId, string sku, int quantity, CancellationToken cancellationToken = default)
    {
        // TODO: Call inventoryApi.ReleaseAsync(), handle errors
        throw new NotImplementedException();
    }
}
