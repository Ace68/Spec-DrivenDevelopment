using SantaClaus.Marketing.SharedKernel.CustomTypes;
using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record WishApproved : DomainEvent
{
    public required ChildId ChildId { get; init; }
    public required DateTimeOffset ApprovedAt { get; init; }
}
