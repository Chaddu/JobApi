using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<List<Notification>> GetByUserIdAsync(int userId);
    Task<List<Notification>> GetUnreadByUserIdAsync(int userId);
}
