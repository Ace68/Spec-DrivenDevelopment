using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record ChildBehaviorUpdated : DomainEvent
{
    public int NewBehaviorScore { get; init; }
}
