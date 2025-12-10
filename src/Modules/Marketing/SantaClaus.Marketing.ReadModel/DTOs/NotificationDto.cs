namespace SantaClaus.Marketing.ReadModel.DTOs;

/// <summary>
/// Data Transfer Object for Notification read model.
/// </summary>
public sealed record NotificationDto(
    string NotificationId,
    string ChildId,
    string Type,
    string Message,
    string Channel,
    DateTime SentAt,
    string Status);

/// <summary>
/// Simplified Notification DTO for list items.
/// </summary>
public sealed record NotificationListItemDto(
    string NotificationId,
    string Type,
    DateTime SentAt,
    string Status);

/// <summary>
/// Response containing notification history.
/// </summary>
public sealed record NotificationHistoryResponseDto(
    string ChildId,
    IReadOnlyList<NotificationListItemDto> Notifications);
