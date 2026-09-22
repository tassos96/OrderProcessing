using System.Net;
using System.Net.Http.Json;
using BuildingBlocks.Application.Envelope;
using FluentAssertions;
using OrderProcessing.IntegrationTests.Fixtures;
using Orders.Application.DTOs;
using Xunit;

namespace OrderProcessing.IntegrationTests.Orders;

public class OrdersApiTests(OrderProcessingWebApplicationFactory factory) : IntegrationTestBase(factory)
{
    [Fact(Skip = "Requires test infrastructure setup — see TODO in WebApplicationFactory")]
    public async Task GetOrder_WithNonExistentId_ShouldReturn404WithEnvelope()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        // Act
        var response = await Client.GetAsync($"/api/orders/{orderId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var envelope = await response.Content.ReadFromJsonAsync<ResponseEnvelope<OrderDto>>();
        envelope.Should().NotBeNull();
        envelope!.Exception.Should().NotBeNull();
        envelope.Exception!.Code.Should().Be("ORDER_NOT_FOUND");
    }

    // TODO: Add test for POST /api/orders with valid request
    // TODO: Add test for POST /api/orders/{id}/cancel
    // TODO: Consider using Testcontainers for SQL Server integration tests
}
