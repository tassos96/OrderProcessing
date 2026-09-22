using BuildingBlocks.Domain.Abstractions;

namespace BuildingBlocks.Contracts;

public interface IIntegrationEvent : IDomainEvent
{
    Guid EventId { get; }
}
