using SantaClaus.Marketing.ReadModel.DTOs;

namespace SantaClaus.Marketing.ReadModel;

/// <summary>
/// Interface for in-memory read model storage and queries.
/// Provides thread-safe storage for DTOs (read models) used in queries.
/// </summary>
public interface IReadModelStore
{
    #region Letter Operations

    /// <summary>
    /// Add or update a letter in the read model store.
    /// </summary>
    void UpsertLetter(LetterDto letter);

    /// <summary>
    /// Get a letter by ID.
    /// </summary>
    LetterDto? GetLetterById(Guid letterId);

    /// <summary>
    /// Get all letters for a specific child with optional filtering.
    /// </summary>
    IReadOnlyList<LetterListItemDto> GetLettersByChild(
        Guid childId,
        string? status = null,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Get total count of letters for a child with optional status filter.
    /// </summary>
    int GetLetterCountByChild(Guid childId, string? status = null);

    #endregion

    #region Child Operations

    /// <summary>
    /// Add or update a child in the read model store.
    /// </summary>
    void UpsertChild(ChildDto child);

    /// <summary>
    /// Get a child by ID.
    /// </summary>
    ChildDto? GetChildById(Guid childId);

    /// <summary>
    /// Get all children with optional filtering.
    /// </summary>
    IReadOnlyList<ChildListItemDto> GetChildren(
        string? country = null,
        int? minBehavior = null,
        int page = 1,
        int pageSize = 20);

    /// <summary>
    /// Get total count of children with optional filters.
    /// </summary>
    int GetChildrenCount(string? country = null, int? minBehavior = null);

    #endregion

    #region Wish Operations

    /// <summary>
    /// Add or update a wish in the read model store.
    /// </summary>
    void UpsertWish(Guid childId, WishItemDto wish);

    /// <summary>
    /// Get all wishes for a specific child.
    /// </summary>
    IReadOnlyList<WishItemDto> GetWishesByChild(Guid childId);

    /// <summary>
    /// Get a specific wish by ID.
    /// </summary>
    WishItemDto? GetWishById(Guid wishId);

    #endregion
}
