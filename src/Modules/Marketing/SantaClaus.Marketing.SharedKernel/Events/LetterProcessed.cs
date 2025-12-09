using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record LetterProcessed : DomainEvent
{
    public DateTimeOffset ProcessedAt { get; init; }
}
