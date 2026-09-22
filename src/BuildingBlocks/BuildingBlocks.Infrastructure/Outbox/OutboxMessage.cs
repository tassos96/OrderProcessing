namespace BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Domain event → Outbox → Background publisher → Message broker.
/// This provides reliable event publication after database transactions.
/// </summary>
public sealed class OutboxMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Type { get; init; }
    public required string Content { get; init; }
    public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}
