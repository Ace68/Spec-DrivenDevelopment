using SantaClaus.Shared.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record WishRejected : DomainEvent
{
    public required ChildId ChildId { get; init; }
    public required DateTimeOffset RejectedAt { get; init; }
    public required RejectionReason Reason { get; init; }
}
