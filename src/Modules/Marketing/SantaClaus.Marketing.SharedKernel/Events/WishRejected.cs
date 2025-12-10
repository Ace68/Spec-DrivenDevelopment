using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record WishRejected : DomainEvent
{
    public required DateTimeOffset RejectedAt { get; init; }
    public required string Reason { get; init; }
}
