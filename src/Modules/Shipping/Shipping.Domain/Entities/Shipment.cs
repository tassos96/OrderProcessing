using BuildingBlocks.Domain.Abstractions;
using Shipping.Domain.Events;
using Shipping.Domain.Exceptions;
using Shipping.Domain.ValueObjects;

namespace Shipping.Domain.Entities;

public sealed class Shipment : AggregateRoot<Guid>
{
    public Guid OrderId { get; private set; }
    public TrackingNumber? TrackingNumber { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; } = default!;
    public DateTimeOffset? EstimatedDelivery { get; private set; }

    private Shipment() { }

    public static Shipment Create(Guid orderId, Address address)
    {
        var shipment = new Shipment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ShippingAddress = address,
            Status = ShipmentStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        shipment.AddDomainEvent(new ShipmentCreatedDomainEvent(shipment.Id, orderId, DateTime.UtcNow));
        return shipment;
    }

    public void MarkShipped(TrackingNumber trackingNumber)
    {
        if (Status != ShipmentStatus.LabelCreated && Status != ShipmentStatus.Pending)
            throw new ShippingDomainException($"Cannot ship from status '{Status}'.");

        TrackingNumber = trackingNumber;
        Status = ShipmentStatus.Shipped;
    }

    public void MarkDelivered()
    {
        if (Status is not (ShipmentStatus.Shipped or ShipmentStatus.InTransit))
            throw new ShippingDomainException($"Cannot mark delivered from status '{Status}'.");

        Status = ShipmentStatus.Delivered;
        // TODO: Raise ShipmentDeliveredDomainEvent
    }
}
