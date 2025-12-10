namespace SantaClaus.Marketing.ReadModel.DTOs;

/// <summary>
/// Data Transfer Object for Notification read model.
/// </summary>
public sealed record NotificationDto(
    Guid NotificationId,
    Guid ChildId,
    string Type,
    string Message,
    string Channel,
    DateTime SentAt,
    string Status);

/// <summary>
/// Simplified Notification DTO for list items.
/// </summary>
public sealed record NotificationListItemDto(
    Guid NotificationId,
    string Type,
    DateTime SentAt,
    string Status);

/// <summary>
/// Response containing notification history.
/// </summary>
public sealed record NotificationHistoryResponseDto(
    Guid ChildId,
    IReadOnlyList<NotificationListItemDto> Notifications);
