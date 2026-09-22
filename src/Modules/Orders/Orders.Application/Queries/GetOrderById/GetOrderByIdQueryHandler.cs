using MediatR;
using Orders.Application.DTOs;
using Orders.Domain.Repositories;

namespace Orders.Application.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler(
    IOrderRepository orderRepository) : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query:
        // 1. Load order from repository
        // 2. Map to OrderDto
        // 3. Return null if not found
        throw new NotImplementedException();
    }
}
