using Inventory.Domain.Entities;
using Inventory.Domain.Repositories;
using Inventory.Domain.ValueObjects;

namespace Inventory.Infrastructure.Persistence.Repositories;

public sealed class InventoryRepository(InventoryDbContext context) : IInventoryRepository
{
    public Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public Task<InventoryItem?> GetBySkuAsync(Sku sku, CancellationToken cancellationToken = default)
    {
        // TODO: Implement with EF Core query on owned property
        throw new NotImplementedException();
    }

    public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        await context.InventoryItems.AddAsync(item, cancellationToken);
    }

    public void Update(InventoryItem item)
    {
        context.InventoryItems.Update(item);
    }
}
