using Inventory.Domain.Entities;
using Inventory.Domain.ValueObjects;

namespace Inventory.Domain.Repositories;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetBySkuAsync(Sku sku, CancellationToken cancellationToken = default);
    Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default);
    void Update(InventoryItem item);
}
