using BuildingBlocks.Domain.Exceptions;

namespace Pricing.Domain.Exceptions;

public sealed class PricingDomainException : DomainException
{
    public PricingDomainException(string message) : base(message) { }
}
