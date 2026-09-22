using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace OrderProcessing.ArchitectureTests;

public class DomainDependencyTests
{
    private static readonly Assembly OrdersDomain = typeof(Orders.Domain.Entities.Order).Assembly;
    private static readonly Assembly InventoryDomain = typeof(Inventory.Domain.Entities.InventoryItem).Assembly;
    private static readonly Assembly PricingDomain = typeof(Pricing.Domain.Entities.PriceRule).Assembly;
    private static readonly Assembly PaymentsDomain = typeof(Payments.Domain.Entities.Payment).Assembly;
    private static readonly Assembly ShippingDomain = typeof(Shipping.Domain.Entities.Shipment).Assembly;

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    public void Domain_ShouldNotReference_Infrastructure(Assembly domainAssembly)
    {
        var result = Types.InAssembly(domainAssembly)
            .Should()
            .NotHaveDependencyOnAny("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{domainAssembly.GetName().Name} should not reference Entity Framework Core");
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    public void Domain_ShouldNotReference_AspNetCore(Assembly domainAssembly)
    {
        var result = Types.InAssembly(domainAssembly)
            .Should()
            .NotHaveDependencyOnAny("Microsoft.AspNetCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{domainAssembly.GetName().Name} should not reference ASP.NET Core");
    }

    [Theory]
    [MemberData(nameof(DomainAssemblies))]
    public void Domain_ShouldNotReference_Refit(Assembly domainAssembly)
    {
        var result = Types.InAssembly(domainAssembly)
            .Should()
            .NotHaveDependencyOnAny("Refit")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{domainAssembly.GetName().Name} should not reference Refit");
    }

    public static TheoryData<Assembly> DomainAssemblies => new()
    {
        OrdersDomain,
        InventoryDomain,
        PricingDomain,
        PaymentsDomain,
        ShippingDomain
    };
}
