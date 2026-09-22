namespace BuildingBlocks.Application.Envelope;

public sealed record RequestHeaders
{
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Guid RequestId { get; init; } = Guid.NewGuid();
    public string? Source { get; init; }
    public string? Channel { get; init; }
}
