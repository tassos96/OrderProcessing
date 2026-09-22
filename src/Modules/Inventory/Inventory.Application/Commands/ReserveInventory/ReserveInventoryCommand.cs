using MediatR;

namespace Inventory.Application.Commands.ReserveInventory;

public sealed record ReserveInventoryCommand(
    Guid OrderId,
    string Sku,
    int Quantity) : IRequest<bool>;
