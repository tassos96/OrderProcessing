using BuildingBlocks.Domain.Exceptions;

namespace Payments.Domain.Exceptions;

public sealed class PaymentDomainException : DomainException
{
    public PaymentDomainException(string message) : base(message) { }
}
