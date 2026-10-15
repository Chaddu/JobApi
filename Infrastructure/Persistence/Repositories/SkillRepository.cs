using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class SkillRepository(AppDbContext context)
    : GenericRepository<Skill>(context), ISkillRepository
{
    public async Task<Skill?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(skill => skill.Name == name);
    }

    public async Task<bool> NameExistsAsync(string name)
    {
        return await _dbSet.AnyAsync(skill => skill.Name == name);
    }
}
