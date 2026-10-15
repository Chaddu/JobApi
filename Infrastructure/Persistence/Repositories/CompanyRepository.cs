using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class CompanyRepository(AppDbContext context)
    : GenericRepository<Company>(context), ICompanyRepository
{
    public async Task<bool> NameExistsAsync(string name)
    {
        return await _dbSet.AnyAsync(company => company.Name == name);
    }
}