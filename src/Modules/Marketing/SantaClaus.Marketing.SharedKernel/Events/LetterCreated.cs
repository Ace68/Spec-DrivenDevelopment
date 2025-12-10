using Muflone.Messages.Events;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Events;

public sealed class LetterCreated(
    LetterId aggregateId,
    ChildId childId,
    LetterContent content,
    DateTime receiveDate,
    LetterLanguage letterLanguage) : DomainEvent(aggregateId)
{
    public ChildId ChildId { get; private set; } = childId;
    public LetterContent Content { get; private set; } = content;
    public DateTime ReceivedDate { get; private set; } = receiveDate;
    public LetterLanguage Language { get; private set; } = letterLanguage;
}

