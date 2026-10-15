using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface ICompanyMemberRepository : IGenericRepository<CompanyMember>
{
    Task<CompanyMember?> GetMembershipAsync(int companyId, int userId);
    Task<bool> IsMemberAsync(int companyId, int userId);
    Task<List<CompanyMember>> GetByCompanyIdAsync(int companyId);
    Task<List<CompanyMember>> GetByUserIdAsync(int userId);
}
