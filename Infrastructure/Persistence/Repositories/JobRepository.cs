using Application.DTOs.Request;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal class JobRepository(AppDbContext context) : GenericRepository<Job>(context), IJobRepository
{
    public async Task<List<Job>> GetByCompanyIdAsync(int companyId)
    {
        return await _dbSet.Where(j => j.CompanyId == companyId)
                       .ToListAsync();
    }

    public async Task<List<Job>> GetByCreatorIdAsync(int userId)
    {
        return await _dbSet.Where(j => j.CreatedById == userId)
                       .ToListAsync();
    }

    public async Task<List<Job>> GetExpiredOpenJobsAsync(DateTime utcNow)
    {
        return await _dbSet
            .Where(job =>
                job.Status == JobStatus.Open &&
                job.ApplicationDeadline.HasValue &&
                job.ApplicationDeadline.Value <= utcNow)
            .ToListAsync();
    }

    public async Task<bool> NameExistsAsync(int companyId, string title)
    {
        return await _dbSet.AnyAsync(j => j.CompanyId == companyId && j.Title == title);
    }

    public async Task<(List<Job> Items, int TotalCount)> SearchAsync(JobFilterRequest request)
    {
        IQueryable<Job> query = _dbSet.Where(j => j.Status == JobStatus.Open);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x => x.Title.Contains(request.Search) || x.Description.Contains(request.Search));
        }
        if(!string.IsNullOrWhiteSpace(request.Location))
        {
            query = query.Where(x => x.Location.Contains(request.Location));
        }
        if(request.EmploymentType.HasValue)
        {
            query = query.Where(x => x.EmploymentType == request.EmploymentType.Value);
        }

        if (request.ExperienceLevel.HasValue)
        {
            query = query.Where(x => x.ExperienceLevel == request.ExperienceLevel.Value);
        }

        if(request.MinSalary.HasValue)
        {
            query = query.Where(x => x.SalaryMin >= request.MinSalary.Value);
        }

        if(request.MaxSalary.HasValue)
        {
            query = query.Where(x => x.SalaryMax <= request.MaxSalary.Value);
        }

        if(!string.IsNullOrWhiteSpace(request.Skill))
        {
            query = query.Where(x => x.Description.Contains(request.Skill));
        }

        var totalCount = await query.CountAsync();

        query = request.SortBy.ToLower() switch
        {
            "title" => request.SortDirection.ToLower() == "asc"
                ? query.OrderBy(job => job.Title)
                : query.OrderByDescending(job => job.Title),

            "salarymin" => request.SortDirection.ToLower() == "asc"
                ? query.OrderBy(job => job.SalaryMin)
                : query.OrderByDescending(job => job.SalaryMin),

            "applicationdeadline" => request.SortDirection.ToLower() == "asc"
                ? query.OrderBy(job => job.ApplicationDeadline)
                : query.OrderByDescending(job => job.ApplicationDeadline),

            _ => request.SortDirection.ToLower() == "asc"
                ? query.OrderBy(job => job.CreatedAt)
                : query.OrderByDescending(job => job.CreatedAt)
        };

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }


}
