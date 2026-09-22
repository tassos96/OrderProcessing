using BuildingBlocks.Application.Abstractions;
using MediatR;
using Orders.Domain.Repositories;

namespace Orders.Application.Commands.CancelOrder;

public sealed class CancelOrderCommandHandler(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CancelOrderCommand>
{
    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement cancel orchestration:
        // 1. Load order
        // 2. Call order.Cancel(reason) — domain enforces valid state transition
        // 3. Release inventory reservations
        // 4. Initiate payment refund if payment was taken
        // 5. Persist changes
        throw new NotImplementedException();
    }
}
