using MediatR;

namespace BuildingBlocks.Domain.Abstractions;

public interface IDomainEvent : INotification
{
    DateTime OccurredOnUtc { get; }
}
