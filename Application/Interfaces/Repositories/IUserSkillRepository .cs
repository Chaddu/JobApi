using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface IUserSkillRepository : IGenericRepository<UserSkill>
{
    Task<UserSkill?> GetByUserAndSkillAsync(int userId, int skillId);
    Task<List<UserSkill>> GetByUserIdAsync(int userId);
}