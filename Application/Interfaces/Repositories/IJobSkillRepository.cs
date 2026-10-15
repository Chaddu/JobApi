using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface IJobSkillRepository : IGenericRepository<JobSkill>
{
    Task<JobSkill?> GetByJobAndSkillAsync(int jobId, int skillId);
    Task<List<JobSkill>> GetByJobIdAsync(int jobId);

}
