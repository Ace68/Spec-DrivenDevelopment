using Muflone.Core;
using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record WishCreated : DomainEvent
{
    public required Guid ChildId { get; init; }
    public required string ToyDescription { get; init; }
    public required int Priority { get; init; }
}
