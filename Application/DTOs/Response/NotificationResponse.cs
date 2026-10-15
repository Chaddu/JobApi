using Domain.Enums;

namespace Application.DTOs.Response;

public record NotificationResponse(
    int Id,
    string Title,
    string Message,
    NotificationType Type,
    bool IsRead,
    DateTime CreatedAt);