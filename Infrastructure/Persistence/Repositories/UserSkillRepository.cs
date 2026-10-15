using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserSkillRepository(AppDbContext context)
    : GenericRepository<UserSkill>(context), IUserSkillRepository
{
    public async Task<UserSkill?> GetByUserAndSkillAsync(int userId, int skillId)
    {
        return await _dbSet
            .Include(userSkill => userSkill.Skill)
            .FirstOrDefaultAsync(userSkill =>
                userSkill.UserId == userId &&
                userSkill.SkillId == skillId);
    }

    public async Task<List<UserSkill>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(userSkill => userSkill.Skill)
            .Where(userSkill => userSkill.UserId == userId)
            .ToListAsync();
    }
}
