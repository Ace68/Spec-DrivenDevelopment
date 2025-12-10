namespace SantaClaus.Marketing.ReadModel.DTOs;

/// <summary>
/// Data Transfer Object for a single Wish.
/// </summary>
public sealed record WishItemDto(
    string WishId,
    string Description,
    string Category,
    int Priority,
    string Status);

/// <summary>
/// Data Transfer Object for Child Wishes collection.
/// </summary>
public sealed record WishCollectionDto(
    string ChildId,
    IReadOnlyList<WishItemDto> Wishes);
