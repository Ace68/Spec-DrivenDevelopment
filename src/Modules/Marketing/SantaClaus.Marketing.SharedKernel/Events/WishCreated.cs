using Muflone.Messages.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed class WishCreated(
    ChildId aggregateId,
    ToyDescription toyDescription,
    int priority) : DomainEvent(aggregateId)
{
    public ToyDescription ToyDescription { get; private set; } = toyDescription;
    public int Priority { get; private set; } = priority;
}
