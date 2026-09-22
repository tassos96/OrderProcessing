using FluentAssertions;
using Inventory.Domain.Entities;
using Inventory.Domain.Exceptions;
using Inventory.Domain.ValueObjects;
using Xunit;

namespace OrderProcessing.UnitTests.Inventory;

public class InventoryItemTests
{
    [Fact]
    public void Reserve_WithSufficientStock_ShouldReduceAvailable()
    {
        // Arrange
        var item = InventoryItem.Create(new Sku("SKU-001"), 100);

        // Act
        item.Reserve(10, Guid.NewGuid());

        // Assert
        item.AvailableQuantity.Value.Should().Be(90);
        item.ReservedQuantity.Value.Should().Be(10);
    }

    [Fact]
    public void Reserve_WithInsufficientStock_ShouldThrow()
    {
        // Arrange
        var item = InventoryItem.Create(new Sku("SKU-001"), 5);

        // Act
        var act = () => item.Reserve(10, Guid.NewGuid());

        // Assert
        act.Should().Throw<InventoryDomainException>();
    }
}
