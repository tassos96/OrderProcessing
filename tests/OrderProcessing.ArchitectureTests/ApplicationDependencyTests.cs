using System.Reflection;
using FluentAssertions;
using NetArchTest.Rules;
using Xunit;

namespace OrderProcessing.ArchitectureTests;

public class ApplicationDependencyTests
{
    private static readonly Assembly OrdersApplication = typeof(Orders.Application.Commands.CreateOrder.CreateOrderCommand).Assembly;
    private static readonly Assembly InventoryApplication = typeof(Inventory.Application.Commands.ReserveInventory.ReserveInventoryCommand).Assembly;

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void Application_ShouldNotReference_Infrastructure(Assembly appAssembly)
    {
        var result = Types.InAssembly(appAssembly)
            .Should()
            .NotHaveDependencyOnAny("Microsoft.EntityFrameworkCore")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{appAssembly.GetName().Name} should not reference Entity Framework Core");
    }

    [Theory]
    [MemberData(nameof(ApplicationAssemblies))]
    public void Application_ShouldNotReference_Refit(Assembly appAssembly)
    {
        var result = Types.InAssembly(appAssembly)
            .Should()
            .NotHaveDependencyOnAny("Refit")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(
            $"{appAssembly.GetName().Name} should not reference Refit");
    }

    public static TheoryData<Assembly> ApplicationAssemblies => new()
    {
        OrdersApplication,
        InventoryApplication
    };
}
