using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICompanyReviewRepository : IGenericRepository<CompanyReview>
{
    Task<CompanyReview?> GetByCompanyAndUserAsync(int companyId, int userId);
    Task<List<CompanyReview>> GetByCompanyIdAsync(int companyId);
}
