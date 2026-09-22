using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.ValueObjects;
using Payments.Domain.Events;
using Payments.Domain.Exceptions;
using Payments.Domain.ValueObjects;

namespace Payments.Domain.Entities;

public sealed class Payment : AggregateRoot<Guid>
{
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public PaymentStatus Status { get; private set; }
    public string? TransactionReference { get; private set; }
    public string? FailureReason { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, Money amount)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void MarkProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new PaymentDomainException($"Cannot process payment in status '{Status}'.");
        Status = PaymentStatus.Processing;
    }

    public void Complete(string transactionReference)
    {
        if (Status != PaymentStatus.Processing)
            throw new PaymentDomainException($"Cannot complete payment in status '{Status}'.");

        Status = PaymentStatus.Completed;
        TransactionReference = transactionReference;
        AddDomainEvent(new PaymentCompletedDomainEvent(Id, OrderId, Amount.Amount, DateTime.UtcNow));
    }

    public void Fail(string reason)
    {
        if (Status is PaymentStatus.Completed or PaymentStatus.Refunded)
            throw new PaymentDomainException($"Cannot fail payment in status '{Status}'.");

        Status = PaymentStatus.Failed;
        FailureReason = reason;
        AddDomainEvent(new PaymentFailedDomainEvent(Id, OrderId, reason, DateTime.UtcNow));
    }

    public void Refund()
    {
        // TODO: Implement refund logic
        throw new NotImplementedException();
    }
}
