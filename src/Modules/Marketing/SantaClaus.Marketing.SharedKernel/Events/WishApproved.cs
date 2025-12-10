using Muflone.Messages.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed class WishApproved(ChildId aggregateId, DateTime approvedAt) : DomainEvent(aggregateId)
{
    public DateTime ApprovedAt { get; private set; } = approvedAt;
}
