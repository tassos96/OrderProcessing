using BuildingBlocks.Domain.Exceptions;

namespace Shipping.Domain.Exceptions;

public sealed class ShippingDomainException : DomainException
{
    public ShippingDomainException(string message) : base(message) { }
}
