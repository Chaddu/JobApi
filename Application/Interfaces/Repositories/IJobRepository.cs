using Application.DTOs.Request;
using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface IJobRepository : IGenericRepository<Job>
{
    Task<List<Job>> GetByCompanyIdAsync(int companyId);
    Task<List<Job>> GetByCreatorIdAsync(int userId);
    Task<bool> NameExistsAsync(int companyId, string title);
    Task<(List<Job> Items, int TotalCount)> SearchAsync(JobFilterRequest request);
    Task<List<Job>> GetExpiredOpenJobsAsync(DateTime utcNow);
}
