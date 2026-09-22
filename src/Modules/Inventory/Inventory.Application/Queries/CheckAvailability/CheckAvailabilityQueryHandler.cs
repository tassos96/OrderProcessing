using Inventory.Application.DTOs;
using MediatR;

namespace Inventory.Application.Queries.CheckAvailability;

public sealed class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, InventoryItemDto?>
{
    public Task<InventoryItemDto?> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
    {
        // TODO: Query inventory by SKU, map to DTO
        throw new NotImplementedException();
    }
}
