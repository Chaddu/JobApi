using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SavedJobRepository(AppDbContext context)
    : GenericRepository<SavedJob>(context), ISavedJobRepository
{
    public async Task<SavedJob?> GetByUserAndJobAsync(int userId, int jobId)
    {
        return await _dbSet.FirstOrDefaultAsync(savedJob =>
            savedJob.UserId == userId && savedJob.JobId == jobId);
    }

    public async Task<List<SavedJob>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(savedJob => savedJob.UserId == userId && savedJob.Job.Status == JobStatus.Open)
            .ToListAsync();
    }
}