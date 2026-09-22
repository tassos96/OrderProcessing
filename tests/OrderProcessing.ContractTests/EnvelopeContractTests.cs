using System.Text.Json;
using BuildingBlocks.Application.Envelope;
using FluentAssertions;
using Xunit;

namespace OrderProcessing.ContractTests;

public class EnvelopeContractTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [Fact]
    public void ResponseEnvelope_Success_ShouldHaveNullException()
    {
        // Arrange & Act
        var envelope = ResponseEnvelope<string>.Success("test-value");

        // Assert
        envelope.Payload.Should().Be("test-value");
        envelope.Exception.Should().BeNull();
    }

    [Fact]
    public void ResponseEnvelope_Failure_ShouldHaveNullPayload()
    {
        // Arrange & Act
        var envelope = ResponseEnvelope<string>.Failure("ERR_001", "Something went wrong");

        // Assert
        envelope.Payload.Should().BeNull();
        envelope.Exception.Should().NotBeNull();
        envelope.Exception!.Code.Should().Be("ERR_001");
    }

    [Fact]
    public void RequestEnvelope_ShouldSerializeAndDeserialize()
    {
        // Arrange
        var envelope = new RequestEnvelope<TestPayload>
        {
            Headers = new RequestHeaders
            {
                CorrelationId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Source = "test"
            },
            Payload = new TestPayload("hello")
        };

        // Act
        var json = JsonSerializer.Serialize(envelope, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<RequestEnvelope<TestPayload>>(json, JsonOptions);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Headers.CorrelationId.Should().Be(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        deserialized.Payload.Value.Should().Be("hello");
    }

    private sealed record TestPayload(string Value);
}
