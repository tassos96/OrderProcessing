using MediatR;

namespace Inventory.Application.Commands.ReserveInventory;

public sealed class ReserveInventoryCommandHandler : IRequestHandler<ReserveInventoryCommand, bool>
{
    public Task<bool> Handle(ReserveInventoryCommand request, CancellationToken cancellationToken)
    {
        // TODO: Load inventory item by SKU, call Reserve(), persist
        throw new NotImplementedException();
    }
}
