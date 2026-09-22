using BuildingBlocks.Domain.Exceptions;

namespace Orders.Domain.Exceptions;

public sealed class OrderDomainException : DomainException
{
    public OrderDomainException(string message) : base(message) { }
}
