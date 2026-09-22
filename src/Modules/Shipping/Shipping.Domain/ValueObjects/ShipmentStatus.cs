namespace Shipping.Domain.ValueObjects;

public enum ShipmentStatus
{
    Pending = 0,
    LabelCreated = 1,
    Shipped = 2,
    InTransit = 3,
    Delivered = 4,
    Returned = 5
}
