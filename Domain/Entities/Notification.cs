using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;
public class Notification : BaseEntity<int>
{
    public int UserId { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;

    public NotificationType Type { get; private set; }
    public bool IsRead { get; private set; }

    public User User { get; private set; } = null!;

    private Notification()
    {
    }

    public Notification(
        int userId,
        string title,
        string message,
        NotificationType type)
    {
        UserId = userId;
        Title = title;
        Message = message;
        Type = type;
        IsRead = false;
    }

    public void MarkAsRead(int? updatedById = null)
    {
        if (IsRead)
            return;

        IsRead = true;

        MarkAsUpdated(updatedById);
    }

}
