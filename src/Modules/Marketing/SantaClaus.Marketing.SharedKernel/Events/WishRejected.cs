using Muflone.Messages.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed class WishRejected(ChildId aggregateId, DateTime rejectedAt, string reason) : DomainEvent(aggregateId)
{
    public DateTime RejectedAt { get; private set; } = rejectedAt;
    public string Reason { get; private set; } = reason;
}
