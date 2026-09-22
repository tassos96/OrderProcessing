using System.Net.Http.Json;
using BuildingBlocks.Application.Envelope;
using Xunit;

namespace OrderProcessing.IntegrationTests.Fixtures;

public abstract class IntegrationTestBase : IClassFixture<OrderProcessingWebApplicationFactory>
{
    protected readonly HttpClient Client;

    protected IntegrationTestBase(OrderProcessingWebApplicationFactory factory)
    {
        Client = factory.CreateClient();
    }

    protected static RequestEnvelope<T> WrapInEnvelope<T>(T payload) => new()
    {
        Headers = new RequestHeaders
        {
            CorrelationId = Guid.NewGuid(),
            RequestId = Guid.NewGuid(),
            Source = "IntegrationTest"
        },
        Payload = payload
    };

    protected static async Task<ResponseEnvelope<T>?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        return await response.Content.ReadFromJsonAsync<ResponseEnvelope<T>>();
    }
}
