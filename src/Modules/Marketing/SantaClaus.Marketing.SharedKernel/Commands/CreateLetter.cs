using Muflone.Messages.Commands;
using SantaClaus.Marketing.SharedKernel.CustomTypes;

namespace SantaClaus.Marketing.SharedKernel.Commands;

public sealed class CreateLetter(
    LetterId aggregateId,
    ChildId childId,
    LetterContent letterContent,
    ReceivedDate receivedDate,
    LetterLanguage letterLanguage) : Command(aggregateId)
{
    public ChildId ChildId { get; private set; } = childId;
    public LetterContent Content { get; private set; } = letterContent;
    public ReceivedDate ReceivedDate { get; private set; } = receivedDate;
    public LetterLanguage Language { get; private set; } = letterLanguage;
}
