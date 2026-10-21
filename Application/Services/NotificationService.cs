using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;

namespace Application.Services;

public class NotificationService(IUnitOfWork unitOfWork, IMapper mapper) : INotificationService
{
    public async Task<Result> DeleteAsync(int id, int currentUserId)
    {
        var notification = await unitOfWork.Notifications.GetByIdAsync(id);

        if(notification is null)
            return Result.Failure("Notification not found", ErrorType.NotFound);
        
        if(notification.UserId != currentUserId)
            return Result.Failure("You are not allowed to delete this notification", ErrorType.Forbidden);

        unitOfWork.Notifications.SoftDelete(notification, currentUserId);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Notification deleted successfully.");

    }

    public async Task<Result<List<NotificationResponse>>> GetMyNotificationsAsync(int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if(user is null)
            return Result<List<NotificationResponse>>.Failure("User not found", ErrorType.NotFound);
        
        var notifications = await unitOfWork.Notifications.GetByUserIdAsync(currentUserId);

        var response = mapper.Map<List<NotificationResponse>>(notifications);

        return Result<List<NotificationResponse>>.Success(response);
    }

    public async Task<Result<List<NotificationResponse>>> GetMyUnreadNotificationsAsync(int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if(user is null)
            return Result<List<NotificationResponse>>.Failure("User not found", ErrorType.NotFound);
        
        var notifications = await unitOfWork.Notifications.GetUnreadByUserIdAsync(currentUserId);

        var response = mapper.Map<List<NotificationResponse>>(notifications);

        return Result<List<NotificationResponse>>.Success(response);
    }

    public async Task<Result> MarkAsReadAsync(int id, int currentUserId)
    {
        var notification = await unitOfWork.Notifications.GetByIdAsync(id);

        if (notification is null)
            return Result.Failure("Notification not found", ErrorType.NotFound);

        if(notification.UserId != currentUserId)
            return Result.Failure("You are not allowed to modify this notification", ErrorType.Forbidden);

        notification.MarkAsRead();

        unitOfWork.Notifications.Update(notification);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Notification marked as read.");
    }
}
