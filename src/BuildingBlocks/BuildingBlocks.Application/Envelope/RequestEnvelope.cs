namespace BuildingBlocks.Application.Envelope;

public sealed record RequestEnvelope<T>
{
    public RequestHeaders Headers { get; init; } = new();
    public T Payload { get; init; } = default!;
}
