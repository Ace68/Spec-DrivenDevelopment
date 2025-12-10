using SantaClaus.Shared.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record WishCreated : DomainEvent
{
    public required ChildId ChildId { get; init; }
    public required ToyDescription ToyDescription { get; init; }
    public required WishPriority Priority { get; init; }
}
