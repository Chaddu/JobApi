using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CompanyMemberRepository(AppDbContext context)
    : GenericRepository<CompanyMember>(context), ICompanyMemberRepository
{
    public async Task<CompanyMember?> GetMembershipAsync(int companyId, int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(member =>
            member.CompanyId == companyId && member.UserId == userId);
    }

    public async Task<bool> IsMemberAsync(int companyId, int userId)
    {
        return await _dbSet.AnyAsync(member =>
            member.CompanyId == companyId && member.UserId == userId);
    }

    public async Task<List<CompanyMember>> GetByCompanyIdAsync(int companyId)
    {
        return await _dbSet
            .Where(member => member.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<List<CompanyMember>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Where(member => member.UserId == userId)
            .ToListAsync();
    }
}