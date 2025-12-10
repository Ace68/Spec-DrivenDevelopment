using System.Collections.Concurrent;
using SantaClaus.Marketing.ReadModel.DTOs;

namespace SantaClaus.Marketing.Infrastructure.ReadModels;

/// <summary>
/// Thread-safe in-memory implementation of the read model store.
/// Uses ConcurrentDictionary for thread-safe access to read models.
/// This is suitable for development and testing; production should use a persistent store.
/// </summary>
public sealed class InMemoryReadModelStore : IReadModelStore
{
    // Letter storage: Key = LetterId
    private readonly ConcurrentDictionary<Guid, LetterDto> _letters = new();

    // Child storage: Key = ChildId
    private readonly ConcurrentDictionary<Guid, ChildDto> _children = new();

    // Wish storage: Key = WishId
    private readonly ConcurrentDictionary<Guid, WishItemDto> _wishes = new();

    // Notification storage: Key = NotificationId
    private readonly ConcurrentDictionary<Guid, NotificationDto> _notifications = new();

    #region Letter Operations

    public void UpsertLetter(LetterDto letter)
    {
        ArgumentNullException.ThrowIfNull(letter);
        _letters.AddOrUpdate(letter.LetterId, letter, (_, _) => letter);
    }

    public LetterDto? GetLetterById(Guid letterId)
    {
        return _letters.TryGetValue(letterId, out var letter) ? letter : null;
    }

    public IReadOnlyList<LetterListItemDto> GetLettersByChild(
        Guid childId,
        string? status = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _letters.Values
            .Where(l => l.ChildId == childId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(l => l.Status == status);

        return query
            .OrderByDescending(l => l.ReceivedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new LetterListItemDto(l.LetterId, l.ChildId, l.ReceivedDate, l.Status))
            .ToList()
            .AsReadOnly();
    }

    public int GetLetterCountByChild(Guid childId, string? status = null)
    {
        var query = _letters.Values.Where(l => l.ChildId == childId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(l => l.Status == status);

        return query.Count();
    }

    #endregion

    #region Child Operations

    public void UpsertChild(ChildDto child)
    {
        ArgumentNullException.ThrowIfNull(child);
        _children.AddOrUpdate(child.ChildId, child, (_, _) => child);
    }

    public ChildDto? GetChildById(Guid childId)
    {
        return _children.TryGetValue(childId, out var child) ? child : null;
    }

    public IReadOnlyList<ChildListItemDto> GetChildren(
        string? country = null,
        int? minBehavior = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = _children.Values.AsEnumerable();

        if (!string.IsNullOrEmpty(country))
            query = query.Where(c => c.Address.Country == country);

        if (minBehavior.HasValue)
            query = query.Where(c => c.BehaviorScore >= minBehavior.Value);

        return query
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ChildListItemDto(
                c.ChildId,
                c.FirstName,
                c.LastName,
                c.Address.Country,
                c.BehaviorScore))
            .ToList()
            .AsReadOnly();
    }

    public int GetChildrenCount(string? country = null, int? minBehavior = null)
    {
        var query = _children.Values.AsEnumerable();

        if (!string.IsNullOrEmpty(country))
            query = query.Where(c => c.Address.Country == country);

        if (minBehavior.HasValue)
            query = query.Where(c => c.BehaviorScore >= minBehavior.Value);

        return query.Count();
    }

    #endregion

    #region Wish Operations

    public void UpsertWish(Guid childId, WishItemDto wish)
    {
        ArgumentNullException.ThrowIfNull(wish);
        _wishes.AddOrUpdate(wish.WishId, wish, (_, _) => wish);
    }

    public IReadOnlyList<WishItemDto> GetWishesByChild(Guid childId)
    {
        return _wishes.Values
            .Where(w => _children.TryGetValue(childId, out _)) // Verify child exists
            .OrderByDescending(w => w.Priority)
            .ToList()
            .AsReadOnly();
    }

    public WishItemDto? GetWishById(Guid wishId)
    {
        return _wishes.TryGetValue(wishId, out var wish) ? wish : null;
    }

    #endregion

    #region Notification Operations

    public void AddNotification(NotificationDto notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        _notifications.TryAdd(notification.NotificationId, notification);
    }

    public IReadOnlyList<NotificationListItemDto> GetNotificationsByChild(Guid childId)
    {
        return _notifications.Values
            .Where(n => n.ChildId == childId)
            .OrderByDescending(n => n.SentAt)
            .Select(n => new NotificationListItemDto(n.NotificationId, n.Type, n.SentAt, n.Status))
            .ToList()
            .AsReadOnly();
    }

    #endregion
}
