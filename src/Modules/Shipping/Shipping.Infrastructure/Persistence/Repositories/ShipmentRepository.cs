using Shipping.Domain.Entities;
using Shipping.Domain.Repositories;

namespace Shipping.Infrastructure.Persistence.Repositories;

public sealed class ShipmentRepository(ShippingDbContext context) : IShipmentRepository
{
    public Task<Shipment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public Task<Shipment?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement
        throw new NotImplementedException();
    }

    public async Task AddAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        await context.Shipments.AddAsync(shipment, cancellationToken);
    }

    public void Update(Shipment shipment)
    {
        context.Shipments.Update(shipment);
    }
}
