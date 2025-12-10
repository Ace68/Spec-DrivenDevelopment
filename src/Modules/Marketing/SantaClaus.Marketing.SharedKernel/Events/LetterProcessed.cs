using Muflone.Messages.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed class LetterProcessed(LetterId aggregateId, DateTime processedAt) : DomainEvent(aggregateId)
{
    public DateTime ProcessedAt { get; private set; } = processedAt;
}
