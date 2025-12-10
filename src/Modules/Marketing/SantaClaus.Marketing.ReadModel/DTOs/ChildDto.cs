namespace SantaClaus.Marketing.ReadModel.DTOs;

/// <summary>
/// Data Transfer Object for Child read model.
/// </summary>
public sealed record ChildDto(
    Guid ChildId,
    string FirstName,
    string LastName,
    DateTime DateOfBirth,
    AddressDetailDto Address,
    int BehaviorScore,
    int LetterCount,
    DateTime RegisteredAt);

/// <summary>
/// Simplified Child DTO for list items.
/// </summary>
public sealed record ChildListItemDto(
    Guid ChildId,
    string FirstName,
    string LastName,
    string Country,
    int BehaviorScore);

/// <summary>
/// Paginated response for children.
/// </summary>
public sealed record ChildListResponseDto(
    IReadOnlyList<ChildListItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
