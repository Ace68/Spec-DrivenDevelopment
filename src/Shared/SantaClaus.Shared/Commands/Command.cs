using Muflone.Core;
using Muflone.Messages.Commands;

namespace SantaClaus.Shared.Commands;

public abstract record Command : ICommand
{
    public IDomainId AggregateId { get; init; } = default!;
    public Guid MessageId { get; set; } = Guid.NewGuid();
    public Guid CorrelationId { get; init; } = Guid.NewGuid();
    public Dictionary<string, object> UserProperties { get; set; } = new();
}
