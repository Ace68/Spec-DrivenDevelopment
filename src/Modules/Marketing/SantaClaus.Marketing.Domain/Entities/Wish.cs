using System.Collections;
using Muflone;
using Muflone.Core;
using SantaClaus.Marketing.Domain.Entities;
using SantaClaus.Marketing.SharedKernel.Events;
using SantaClaus.Shared.Exceptions;
using SharedDomainId = SantaClaus.Shared.ValueObjects.DomainId;

namespace SantaClaus.Marketing.Domain.Entities;

public sealed class Wish : IAggregate
{
    IDomainId IAggregate.Id => new SharedDomainId(Id);
    public int Version { get; private set; }

    public Guid Id { get; private set; }
    public Guid ChildId { get; private set; }
    public string ToyDescription { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public WishStatus Status { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public DateTimeOffset? RejectedAt { get; private set; }

    private readonly List<object> _uncommittedEvents = new();

    private Wish() { }

    public static Wish Create(Guid wishId, Guid childId, string toyDescription, int priority)
    {
        if (wishId == Guid.Empty)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(wishId), new[] { "Wish ID cannot be empty" } }
            });
        if (childId == Guid.Empty)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(childId), new[] { "Child ID cannot be empty" } }
            });
        if (string.IsNullOrWhiteSpace(toyDescription))
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(toyDescription), new[] { "Toy description cannot be empty" } }
            });
        if (priority is < 1 or > 5)
            throw new ValidationException(new Dictionary<string, string[]>
            {
                { nameof(priority), new[] { "Priority must be between 1 and 5" } }
            });

        var wish = new Wish();
        wish.RaiseEvent(new WishCreated
        {
            AggregateId = new SharedDomainId(wishId),
            ChildId = childId,
            ToyDescription = toyDescription,
            Priority = priority
        });
        return wish;
    }

    public void Approve()
    {
        if (Status == WishStatus.Approved)
            throw new DomainException("Wish already approved");
        if (Status == WishStatus.Rejected)
            throw new DomainException("Cannot approve a rejected wish");

        RaiseEvent(new WishApproved
        {
            AggregateId = new SharedDomainId(Id),
            ApprovedAt = DateTimeOffset.UtcNow
        });
    }

    public void Reject(string? reason = null)
    {
        if (Status == WishStatus.Rejected)
            throw new DomainException("Wish already rejected");
        if (Status == WishStatus.Approved)
            throw new DomainException("Cannot reject an approved wish");

        RaiseEvent(new WishRejected
        {
            AggregateId = new SharedDomainId(Id),
            RejectedAt = DateTimeOffset.UtcNow,
            Reason = reason ?? string.Empty
        });
    }

    public void ApplyEvent(object @event)
    {
        Version++;
        switch (@event)
        {
            case WishCreated e:
                Apply(e);
                break;
            case WishApproved e:
                Apply(e);
                break;
            case WishRejected e:
                Apply(e);
                break;
        }
    }

    public ICollection GetUncommittedEvents() => _uncommittedEvents;
    public IMemento? GetSnapshot() => null;

    private void RaiseEvent(object @event)
    {
        ApplyEvent(@event);
        _uncommittedEvents.Add(@event);
    }

    private void Apply(WishCreated e)
    {
        Id = Guid.Parse(e.AggregateId.Value);
        ChildId = e.ChildId;
        ToyDescription = e.ToyDescription;
        Priority = e.Priority;
        Status = WishStatus.Created;
    }

    private void Apply(WishApproved e)
    {
        Status = WishStatus.Approved;
        ApprovedAt = e.ApprovedAt;
    }

    private void Apply(WishRejected e)
    {
        Status = WishStatus.Rejected;
        RejectedAt = e.RejectedAt;
    }

    public void ClearUncommittedEvents() => _uncommittedEvents.Clear();
}
