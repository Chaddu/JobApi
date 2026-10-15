using Application.Common;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface INotificationService
{
    Task<Result<List<NotificationResponse>>> GetMyNotificationsAsync(int currentUserId);
    Task<Result<List<NotificationResponse>>> GetMyUnreadNotificationsAsync(int currentUserId);
    Task<Result> MarkAsReadAsync(int id, int currentUserId);
    Task<Result> DeleteAsync(int id, int currentUserId);
}
