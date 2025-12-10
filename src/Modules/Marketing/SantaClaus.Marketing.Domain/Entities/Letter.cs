using System.Collections;
using Muflone;
using Muflone.Core;
using SantaClaus.Marketing.SharedKernel.CustomTypes;
using SantaClaus.Marketing.SharedKernel.Events;
using SantaClaus.Shared.Exceptions;

namespace SantaClaus.Marketing.Domain.Entities;

public class Letter : AggregateRoot
{
    // Domain properties
    private LetterId _id = null!;
    private Guid _childId = Guid.Empty;
    private LetterContent _letterContent = null!;
    private ReceivedDate _receivedDate = null!;
    private LetterLanguage _language = null!;
    private LetterStatus _status = LetterStatus.Processing;
    private DateTime? _processedAt = null!;

    private readonly List<object> _uncommittedEvents = new();

    protected Letter()
    {
    }

    public static Letter Create(LetterId letterId, ChildId childId, LetterContent content, ReceivedDate receivedDate, LetterLanguage language)
    {
        return new Letter(letterId, childId, content, receivedDate, language);
    }

    private Letter(LetterId letterId, ChildId childId, LetterContent content, ReceivedDate receivedDate,
        LetterLanguage language)
    {
        if (string.IsNullOrWhiteSpace(letterId.Value))
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(letterId), ["Letter ID cannot be empty"]}
            });

        var letter = new Letter();
        RaiseEvent(new LetterCreated(letterId, childId, content, receivedDate, language));
    }

    public void MarkAsProcessed()
    {
        if (_status == LetterStatus.Processed)
            throw new DomainException("Letter has already been processed");

        RaiseEvent(new LetterProcessed(_id, DateTime.UtcNow));
    }

    private void Apply(LetterCreated e)
    {
        _id = (LetterId)e.AggregateId;
        _childId = Guid.Parse(e.ChildId.Value);
        _letterContent = e.Content;
        _receivedDate = e.ReceivedDate;
        _language = e.Language;
        _status = LetterStatus.Received;
    }

    private void Apply(LetterProcessed e)
    {
        _status = LetterStatus.Processed;
        _processedAt = e.ProcessedAt;
    }
}
