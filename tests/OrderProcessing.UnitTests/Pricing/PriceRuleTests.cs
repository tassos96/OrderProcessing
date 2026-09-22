using BuildingBlocks.Domain.ValueObjects;
using FluentAssertions;
using Pricing.Domain.Entities;
using Pricing.Domain.ValueObjects;
using Xunit;

namespace OrderProcessing.UnitTests.Pricing;

public class PriceRuleTests
{
    [Fact]
    public void Create_ShouldSetProperties()
    {
        // Arrange & Act
        var rule = PriceRule.Create("SKU-001", new Money(29.99m, "USD"), DiscountType.Percentage, 10);

        // Assert
        rule.Sku.Should().Be("SKU-001");
        rule.BasePrice.Amount.Should().Be(29.99m);
        rule.DiscountType.Should().Be(DiscountType.Percentage);
        rule.DiscountValue.Should().Be(10);
    }
}
