using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record LetterCreated : DomainEvent
{
    public Guid ChildId { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime ReceivedDate { get; init; }
    public string Language { get; init; } = string.Empty;
}
