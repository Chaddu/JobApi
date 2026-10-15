using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class NotificationRepository(AppDbContext context)
    : GenericRepository<Notification>(context), INotificationRepository
{
    public async Task<List<Notification>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(notification => notification.UserId == userId)
            .OrderByDescending(notification => notification.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Notification>> GetUnreadByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(notification =>
                notification.UserId == userId && !notification.IsRead)
            .OrderByDescending(notification => notification.CreatedAt)
            .ToListAsync();
    }
}