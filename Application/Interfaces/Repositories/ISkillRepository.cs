using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface ISkillRepository : IGenericRepository<Skill>
{
    Task<Skill?> GetByNameAsync(string name);
    Task<bool> NameExistsAsync(string name);
}
