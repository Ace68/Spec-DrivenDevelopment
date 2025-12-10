using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record WishApproved : DomainEvent
{
    public required DateTimeOffset ApprovedAt { get; init; }
}
