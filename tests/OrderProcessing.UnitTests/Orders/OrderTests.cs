using FluentAssertions;
using Orders.Domain.Entities;
using Orders.Domain.Events;
using Orders.Domain.Exceptions;
using Orders.Domain.ValueObjects;
using Xunit;

namespace OrderProcessing.UnitTests.Orders;

public class OrderTests
{
    [Fact]
    public void Create_WithValidData_ShouldRaiseDomainEvent()
    {
        // Arrange
        var customerId = new CustomerId(Guid.NewGuid());
        var address = new Address("123 Main St", "Springfield", "IL", "62701", "US");
        var items = new List<OrderItem>
        {
            new("SKU-001", "Widget", 2, 9.99m)
        };

        // Act
        var order = Order.Create(customerId, address, items);

        // Assert
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(19.98m);
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderCreatedDomainEvent>();
    }

    [Fact]
    public void Cancel_WhenPending_ShouldTransitionToCancelled()
    {
        // Arrange
        var customerId = new CustomerId(Guid.NewGuid());
        var address = new Address("123 Main St", "Springfield", "IL", "62701", "US");
        var order = Order.Create(customerId, address, [new("SKU-001", "Widget", 1, 10m)]);
        order.ClearDomainEvents();

        // Act
        order.Cancel("Customer requested cancellation");

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.CancellationReason.Should().Be("Customer requested cancellation");
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderCancelledDomainEvent>();
    }

    [Fact]
    public void Confirm_WhenNotPaymentPending_ShouldThrow()
    {
        // Arrange
        var customerId = new CustomerId(Guid.NewGuid());
        var address = new Address("123 Main St", "Springfield", "IL", "62701", "US");
        var order = Order.Create(customerId, address, [new("SKU-001", "Widget", 1, 10m)]);

        // Act
        var act = () => order.Confirm();

        // Assert
        act.Should().Throw<OrderDomainException>();
    }
}
