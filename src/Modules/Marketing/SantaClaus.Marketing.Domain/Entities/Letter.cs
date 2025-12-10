using System.Collections;
using Muflone;
using Muflone.Core;
using SantaClaus.Marketing.SharedKernel.CustomTypes;
using SantaClaus.Marketing.SharedKernel.Events;
using SantaClaus.Shared.Exceptions;

namespace SantaClaus.Marketing.Domain.Entities;

public sealed class Letter : IAggregate
{
    // IAggregate.Id - must return IDomainId
    IDomainId IAggregate.Id => Id;
    
    // IAggregate.Version
    public int Version { get; private set; }
    
    // Domain properties
    public LetterId Id { get; private set; } = new(Guid.Empty);
    public Guid ChildId { get; private set; }
    public LetterContent Content { get; private set; } = new(string.Empty);
    public DateTime ReceivedDate { get; private set; }
    public LetterLanguage Language { get; private set; } = new(string.Empty);
    public LetterStatus Status { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private readonly List<object> _uncommittedEvents = new();

    private Letter()
    {
    }

    public static Letter Create(Guid letterId, Guid childId, LetterContent content, DateTime receivedDate, LetterLanguage language)
    {
        if (letterId == Guid.Empty)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(letterId), new[] { "Letter ID cannot be empty" } }
            });

        if (childId == Guid.Empty)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(childId), new[] { "Child ID cannot be empty" } }
            });

        if (string.IsNullOrWhiteSpace(content))
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(content), new[] { "Content cannot be empty" } }
            });

        if (string.IsNullOrWhiteSpace(language))
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(language), new[] { "Language cannot be empty" } }
            });

        var letter = new Letter();
        letter.RaiseEvent(new LetterCreated
        {
            AggregateId = new LetterId(letterId),
            ChildId = childId,
            Content = content,
            ReceivedDate = receivedDate,
            Language = language
        });
        return letter;
    }

    public void MarkAsProcessed()
    {
        if (Status == LetterStatus.Processed)
            throw new DomainException("Letter has already been processed");

        RaiseEvent(new LetterProcessed
        {
            AggregateId = Id,
            ProcessedAt = DateTimeOffset.UtcNow
        });
    }

    private void RaiseEvent(object @event)
    {
        ApplyEvent(@event);
        _uncommittedEvents.Add(@event);
    }

    // IAggregate.ApplyEvent - must be public
    public void ApplyEvent(object @event)
    {
        Version++;
        switch (@event)
        {
            case LetterCreated e:
                Apply(e);
                break;
            case LetterProcessed e:
                Apply(e);
                break;
        }
    }

    // IAggregate.GetUncommittedEvents
    public ICollection GetUncommittedEvents() => _uncommittedEvents;

    // IAggregate.GetSnapshot - not implementing snapshots yet
    public IMemento? GetSnapshot() => null;

    private void Apply(LetterCreated e)
    {
        Id = new LetterId(Guid.Parse(e.AggregateId.Value));
        ChildId = e.ChildId;
        Content = e.Content;
        ReceivedDate = e.ReceivedDate;
        Language = e.Language;
        Status = LetterStatus.Received;
    }

    private void Apply(LetterProcessed e)
    {
        Status = LetterStatus.Processed;
        ProcessedAt = e.ProcessedAt.DateTime;
    }

    public void ClearUncommittedEvents()
    {
        _uncommittedEvents.Clear();
    }
}
