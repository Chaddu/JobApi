using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CompanyReviewRepository(AppDbContext context)
    : GenericRepository<CompanyReview>(context), ICompanyReviewRepository
{
    public async Task<CompanyReview?> GetByCompanyAndUserAsync(
        int companyId, int userId)
    {
        return await _dbSet
            .Include(review => review.User)
            .FirstOrDefaultAsync(review =>
                review.CompanyId == companyId &&
                review.UserId == userId);
    }

    public async Task<List<CompanyReview>> GetByCompanyIdAsync(int companyId)
    {
        return await _dbSet
            .Include(review => review.User)
            .Where(review => review.CompanyId == companyId)
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync();
    }
}
