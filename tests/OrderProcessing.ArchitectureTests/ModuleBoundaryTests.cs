using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace OrderProcessing.ArchitectureTests;

public class ModuleBoundaryTests
{
    private static readonly Assembly OrdersDomain = typeof(Orders.Domain.Entities.Order).Assembly;

    [Fact]
    public void OrdersDomain_ShouldNotReference_InventoryDomain()
    {
        var result = Types.InAssembly(OrdersDomain)
            .Should()
            .NotHaveDependencyOnAny("Inventory.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            "Orders.Domain must not directly reference Inventory.Domain — use contracts for cross-module communication");
    }

    [Fact]
    public void OrdersDomain_ShouldNotReference_PaymentsDomain()
    {
        var result = Types.InAssembly(OrdersDomain)
            .Should()
            .NotHaveDependencyOnAny("Payments.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void OrdersDomain_ShouldNotReference_ShippingDomain()
    {
        var result = Types.InAssembly(OrdersDomain)
            .Should()
            .NotHaveDependencyOnAny("Shipping.Domain")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void OrdersDomain_ShouldNotReference_AnyInfrastructure()
    {
        var result = Types.InAssembly(OrdersDomain)
            .Should()
            .NotHaveDependencyOnAny(
                "Orders.Infrastructure",
                "Inventory.Infrastructure",
                "Payments.Infrastructure",
                "Shipping.Infrastructure",
                "Pricing.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
