using Orders.Domain.Entities;
using Orders.Domain.Repositories;
using Orders.Domain.ValueObjects;

namespace Orders.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(OrdersDbContext context) : IOrderRepository
{
    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // TODO: Implement with Include for Items and eager loading
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<Order>> GetByCustomerIdAsync(CustomerId customerId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement with filtering by CustomerId value
        throw new NotImplementedException();
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await context.Orders.AddAsync(order, cancellationToken);
    }

    public void Update(Order order)
    {
        context.Orders.Update(order);
    }
}
