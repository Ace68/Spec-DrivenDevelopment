using SantaClaus.Marketing.SharedKernel.CustomTypes;
using SantaClaus.Shared.Events;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed record LetterCreated : DomainEvent
{
    public Guid ChildId { get; init; }
    public LetterContent Content { get; init; } = new(string.Empty);
    public DateTime ReceivedDate { get; init; }
    public LetterLanguage Language { get; init; } = new(string.Empty);
}

