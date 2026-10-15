using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class JobSkillRepository(AppDbContext context)
    : GenericRepository<JobSkill>(context), IJobSkillRepository
{
    public async Task<JobSkill?> GetByJobAndSkillAsync(int jobId, int skillId)
    {
        return await _dbSet
            .Include(jobSkill => jobSkill.Skill)
            .FirstOrDefaultAsync(jobSkill =>
                jobSkill.JobId == jobId &&
                jobSkill.SkillId == skillId);
    }

    public async Task<List<JobSkill>> GetByJobIdAsync(int jobId)
    {
        return await _dbSet
            .Include(jobSkill => jobSkill.Skill)
            .Where(jobSkill => jobSkill.JobId == jobId)
            .ToListAsync();
    }
}