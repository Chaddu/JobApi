using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class JobApplicationRepository(AppDbContext context)
    : GenericRepository<JobApplication>(context), IJobApplicationRepository
{
    public async Task<bool> HasAppliedAsync(int jobId, int applicantId)
    {
        return await _dbSet.AnyAsync(application =>
            application.JobId == jobId &&
            application.ApplicantId == applicantId);
    }

    public async Task<List<JobApplication>> GetByJobIdAsync(int jobId)
    {
        return await _dbSet
            .Where(application => application.JobId == jobId)
            .ToListAsync();
    }

    public async Task<List<JobApplication>> GetByApplicantIdAsync(int applicantId)
    {
        return await _dbSet
            .Where(application => application.ApplicantId == applicantId)
            .ToListAsync();
    }

    public async Task<bool> HasAcceptedApplicationAsync(int companyId, int userId)
    {
        return await (
            from application in _context.JobApplications
            join job in _context.Jobs on application.JobId equals job.Id
            where job.CompanyId == companyId
                && application.ApplicantId == userId
                && application.Status == ApplicationStatus.Accepted
            select application.Id
        ).AnyAsync();
    }
}