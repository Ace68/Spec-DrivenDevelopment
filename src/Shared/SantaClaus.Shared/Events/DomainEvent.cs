using Muflone.Messages.Events;

namespace SantaClaus.Shared.Events;

public abstract record DomainEvent : IDomainEvent
{
    public Guid AggregateId { get; init; }
    public Guid MessageId { get; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; }
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public int AggregateVersion { get; init; }
}
