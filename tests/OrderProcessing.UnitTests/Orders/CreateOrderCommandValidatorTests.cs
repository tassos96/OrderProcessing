using FluentAssertions;
using Orders.Application.Commands.CreateOrder;
using Orders.Application.DTOs;
using Xunit;

namespace OrderProcessing.UnitTests.Orders;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_WithEmptyCustomerId_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.Empty,
            [new CreateOrderItemDto("SKU-001", "Widget", 1, 10m)],
            new AddressDto("123 Main", "City", "ST", "12345", "US"));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerId");
    }

    [Fact]
    public void Validate_WithNoItems_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            [],
            new AddressDto("123 Main", "City", "ST", "12345", "US"));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }
}
