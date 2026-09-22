namespace BuildingBlocks.Infrastructure.Outbox;

/// <summary>
/// Background service reads unprocessed OutboxMessages, publishes to broker, marks processed.
/// </summary>
public interface IOutboxProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken = default);
}
