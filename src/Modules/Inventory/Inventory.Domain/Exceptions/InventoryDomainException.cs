using BuildingBlocks.Domain.Exceptions;

namespace Inventory.Domain.Exceptions;

public sealed class InventoryDomainException : DomainException
{
    public InventoryDomainException(string message) : base(message) { }
}
