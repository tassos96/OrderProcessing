namespace Orders.Domain.ValueObjects;

public enum OrderStatus
{
    Pending = 0,
    InventoryValidated = 1,
    PaymentPending = 2,
    Confirmed = 3,
    Shipped = 4,
    Completed = 5,
    CancellationRequested = 6,
    Cancelled = 7
}
