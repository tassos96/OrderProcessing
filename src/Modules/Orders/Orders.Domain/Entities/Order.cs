using BuildingBlocks.Domain.Abstractions;
using Orders.Domain.Events;
using Orders.Domain.Exceptions;
using Orders.Domain.ValueObjects;

namespace Orders.Domain.Entities;

public sealed class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = [];

    public CustomerId CustomerId { get; private set; } = default!;
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; } = default!;
    public decimal TotalAmount { get; private set; }
    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public DateTime? ConfirmedAtUtc { get; private set; }
    public DateTime? CancelledAtUtc { get; private set; }
    public string? CancellationReason { get; private set; }

    private Order() { }

    public static Order Create(CustomerId customerId, Address shippingAddress, IEnumerable<OrderItem> items)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            ShippingAddress = shippingAddress,
            Status = OrderStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        order._items.AddRange(items);
        order.TotalAmount = order._items.Sum(i => i.TotalPrice);
        order.AddDomainEvent(new OrderCreatedDomainEvent(order.Id, customerId.Value, DateTime.UtcNow));

        return order;
    }

    public void MarkInventoryValidated()
    {
        if (Status != OrderStatus.Pending)
            throw new OrderDomainException($"Cannot validate inventory for order in status '{Status}'.");

        Status = OrderStatus.InventoryValidated;
        // TODO: Implement state transition logic and raise domain event
    }

    public void MarkPaymentPending()
    {
        if (Status != OrderStatus.InventoryValidated)
            throw new OrderDomainException($"Cannot move to payment pending from status '{Status}'.");

        Status = OrderStatus.PaymentPending;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.PaymentPending)
            throw new OrderDomainException($"Cannot confirm order in status '{Status}'.");

        Status = OrderStatus.Confirmed;
        ConfirmedAtUtc = DateTime.UtcNow;
        AddDomainEvent(new OrderConfirmedDomainEvent(Id, DateTime.UtcNow));
    }

    public void Ship()
    {
        if (Status != OrderStatus.Confirmed)
            throw new OrderDomainException($"Cannot ship order in status '{Status}'.");

        Status = OrderStatus.Shipped;
        // TODO: Implement shipping transition logic
    }

    public void Complete()
    {
        if (Status != OrderStatus.Shipped)
            throw new OrderDomainException($"Cannot complete order in status '{Status}'.");

        Status = OrderStatus.Completed;
    }

    public void Cancel(string reason)
    {
        if (Status is OrderStatus.Shipped or OrderStatus.Completed or OrderStatus.Cancelled)
            throw new OrderDomainException($"Cannot cancel order in status '{Status}'.");

        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
        CancelledAtUtc = DateTime.UtcNow;
        AddDomainEvent(new OrderCancelledDomainEvent(Id, reason, DateTime.UtcNow));
    }
}
