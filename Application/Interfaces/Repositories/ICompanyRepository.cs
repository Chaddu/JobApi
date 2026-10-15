using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICompanyRepository : IGenericRepository<Company>
{
    Task<bool> NameExistsAsync(string name);    
}
