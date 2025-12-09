using Muflone.Messages.Commands;

namespace SantaClaus.Shared.Commands;

public abstract record Command : ICommand
{
    public Guid AggregateId { get; init; }
    public Guid MessageId { get; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
}
