using System.Reflection;
using BuildingBlocks.Contracts;
using FluentAssertions;
using Xunit;

namespace OrderProcessing.ContractTests;

public class IntegrationEventContractTests
{
    private static readonly Assembly[] ContractAssemblies =
    [
        typeof(Orders.Contracts.IntegrationEvents.OrderCreatedIntegrationEvent).Assembly,
        typeof(Inventory.Contracts.IntegrationEvents.InventoryReservedIntegrationEvent).Assembly,
        typeof(Payments.Contracts.IntegrationEvents.PaymentCompletedIntegrationEvent).Assembly,
        typeof(Shipping.Contracts.IntegrationEvents.ShipmentCreatedIntegrationEvent).Assembly
    ];

    [Fact]
    public void AllIntegrationEvents_ShouldImplementIIntegrationEvent()
    {
        foreach (var assembly in ContractAssemblies)
        {
            var eventTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("IntegrationEvent") && !t.IsInterface);

            foreach (var type in eventTypes)
            {
                type.Should().Implement<IIntegrationEvent>(
                    $"{type.Name} should implement IIntegrationEvent");
            }
        }
    }

    [Fact]
    public void AllIntegrationEvents_ShouldBeSealed()
    {
        foreach (var assembly in ContractAssemblies)
        {
            var eventTypes = assembly.GetTypes()
                .Where(t => t.Name.EndsWith("IntegrationEvent") && !t.IsInterface);

            foreach (var type in eventTypes)
            {
                type.IsSealed.Should().BeTrue(
                    $"{type.Name} should be sealed");
            }
        }
    }
}
