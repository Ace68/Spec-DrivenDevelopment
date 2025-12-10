using Muflone.Messages.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed class ChildBehaviorUpdated(ChildId aggregateId, int newBehaviorScore) : DomainEvent(aggregateId)
{
    public int NewBehaviorScore { get; private set; } = newBehaviorScore;
}
