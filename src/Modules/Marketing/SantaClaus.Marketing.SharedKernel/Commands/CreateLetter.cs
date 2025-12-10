using Muflone.Messages.Commands;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed class CreateLetter(
    LetterId aggregateId,
    ChildId childId,
    LetterContent content,
    DateTime receivedDate,
    LetterLanguage letterLanguage) : Command(aggregateId)
{
    public ChildId ChildId { get; private set; } = childId;
    public LetterContent Content { get; private set; } = content;
    public DateTime ReceivedDate { get; private set; } = receivedDate;
    public LetterLanguage Language { get; private set; } = letterLanguage;
}
