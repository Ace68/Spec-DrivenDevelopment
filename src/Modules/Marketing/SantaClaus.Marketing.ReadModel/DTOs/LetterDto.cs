namespace SantaClaus.Marketing.ReadModel.DTOs;

/// <summary>
/// Data Transfer Object for Letter read model.
/// </summary>
public sealed record LetterDto(
    string LetterId,
    string ChildId,
    string Content,
    DateTime ReceivedDate,
    string Language,
    string Status,
    DateTime? ProcessedAt);

/// <summary>
/// Simplified DTO for letter list items.
/// </summary>
public sealed record LetterListItemDto(
    string LetterId,
    string ChildId,
    DateTime ReceivedDate,
    string Status);

/// <summary>
/// Paginated response for letters.
/// </summary>
public sealed record LetterListResponseDto(
    IReadOnlyList<LetterListItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize);
