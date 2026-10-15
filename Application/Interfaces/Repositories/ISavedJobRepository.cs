using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface ISavedJobRepository : IGenericRepository<SavedJob>
{
    Task<SavedJob?> GetByUserAndJobAsync(int userId, int jobId);
    Task<List<SavedJob>> GetByUserIdAsync(int userId);
}
