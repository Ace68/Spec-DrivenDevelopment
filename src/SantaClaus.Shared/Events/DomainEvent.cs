using Muflone.Core;
using Muflone.Messages.Events;

namespace SantaClaus.Shared.Events;

public abstract record DomainEvent : IDomainEvent
{
    public required IDomainId AggregateId { get; init; }
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public int Version { get; init; }
    public Dictionary<string, object> UserProperties { get; set; } = new();
}
