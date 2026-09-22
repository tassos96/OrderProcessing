namespace Orders.Application.Abstractions;

public interface IInventoryService
{
    Task<bool> ReserveStockAsync(Guid orderId, string sku, int quantity, CancellationToken cancellationToken = default);
    Task ReleaseStockAsync(Guid orderId, string sku, int quantity, CancellationToken cancellationToken = default);
}
