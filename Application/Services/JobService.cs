using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class JobService(IUnitOfWork unitOfWork, IMapper mapper) : IJobService
{
    public async Task<Result> CloseAsync(int id, int currentUserId)
    {
        var job = await unitOfWork.Jobs.GetByIdAsync(id);

        if (job is null)
            return Result.Failure("Job not found", ErrorType.NotFound);

        if (!await CanManageCompanyAsync(job.CompanyId, currentUserId))
            return Result.Failure("You are not allowed to close this job.", ErrorType.Forbidden);

        try
        {
            job.Close(currentUserId);

            unitOfWork.Jobs.Update(job);
            await unitOfWork.SaveChangesAsync();

            return Result.Success("Job closed successfully.");
        }
        catch(DomainException ex)
        {
            return Result.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<Result<JobResponse>> CreateAsync(CreateJobRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<JobResponse>.Failure("User not found.", ErrorType.NotFound);

        var company = await unitOfWork.Companies.GetByIdAsync(request.CompanyId);

        if (company is null)
            return Result<JobResponse>.Failure("Company not found.", ErrorType.NotFound);

        if (!await CanManageCompanyAsync(request.CompanyId, currentUserId))
            return Result<JobResponse>.Failure("You are not allowed to create jobs for this company.", ErrorType.Forbidden);

        var title = request.Title?.Trim() ?? string.Empty;
        var description = request.Description?.Trim() ?? string.Empty;
        var location = request.Location?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title) ||
           string.IsNullOrWhiteSpace(description) ||
           string.IsNullOrWhiteSpace(location))
            return Result<JobResponse>.Failure("Title, description, and location are required.", ErrorType.Validation);

        if(await unitOfWork.Jobs.NameExistsAsync(request.CompanyId, title))
            return Result<JobResponse>.Failure("A job with the same title already exists for this company.", ErrorType.Conflict);

        try
        {
            var job = new Job(request.CompanyId,
                currentUserId,
                title,
                description,
                location,
                request.SalaryMin,
                request.SalaryMax,
                request.EmploymentType,
                request.ExperienceLevel,
                request.ApplicationDeadline);
            await unitOfWork.Jobs.AddAsync(job);
            await unitOfWork.SaveChangesAsync();

            var response = mapper.Map<JobResponse>(job);

            return Result<JobResponse>.Success(response);
        }
        catch(DomainException ex)
        {
            return Result<JobResponse>.Failure(ex.Message, ErrorType.Validation);
        }
    }

    public async Task<Result<List<JobResponse>>> GetByCompanyIdAsync(int companyId, int currentUserId)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(companyId);

        if (company is null)
            return Result<List<JobResponse>>.Failure("Company not found.", ErrorType.NotFound);

        if(!await CanManageCompanyAsync(companyId, currentUserId))
            return Result<List<JobResponse>>.Failure("You do not have permission to view jobs for this company.", ErrorType.Forbidden);

        var joobs = await unitOfWork.Jobs.GetByCompanyIdAsync(companyId);

        return Result<List<JobResponse>>.Success(mapper.Map<List<JobResponse>>(joobs));
    }

    public async Task<Result<JobResponse>> GetByIdAsync(int id)
    {
        var job = await unitOfWork.Jobs.GetByIdAsync(id);

        if(job is null || job.Status != JobStatus.Open)
            return Result<JobResponse>.Failure("Job not found.", ErrorType.NotFound);

        var response = mapper.Map<JobResponse>(job);

        return Result<JobResponse>.Success(response);
    }

    public async Task<Result> PublishAsync(int id, int currentUserId)
    {
        var job = await unitOfWork.Jobs.GetByIdAsync(id);

        if(job is null)
            return Result.Failure("Job not found.", ErrorType.NotFound);

        if(!await CanManageCompanyAsync(job.CompanyId, currentUserId))
            return Result.Failure("You are not allowed to publish this job.", ErrorType.Forbidden);

        try
        {
            job.Publish(currentUserId);

            unitOfWork.Jobs.Update(job);
            await unitOfWork.SaveChangesAsync();
            return Result.Success("Job published successfully.");
        }
        catch(DomainException ex)
        {
            return Result.Failure(ex.Message, ErrorType.Validation);
        }

    }

    public async Task<Result<PagedResult<JobResponse>>> SearchAsync(JobFilterRequest request)
    {
        var (items, totalCount) = await unitOfWork.Jobs.SearchAsync(request);

        var page = Math.Max(request.Page, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var response = new PagedResult<JobResponse>
        {
            Items = mapper.Map<List<JobResponse>>(items),
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return Result<PagedResult<JobResponse>>.Success(response);
    }

    public async Task<Result<JobResponse>> UpdateAsync(int id, UpdateJobRequest request, int currentUserId)
    {
        var job = await unitOfWork.Jobs.GetByIdAsync(id);

        if(job is null)
            return Result<JobResponse>.Failure("Job not found.", ErrorType.NotFound);
        
        if(!await CanManageCompanyAsync(job.CompanyId, currentUserId))
            return Result<JobResponse>.Failure("You are not allowed to update this job.", ErrorType.Forbidden);

        var title = request.Title?.Trim() ?? string.Empty;
        var description = request.Description?.Trim() ?? string.Empty;
        var location = request.Location?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(title) ||
          string.IsNullOrWhiteSpace(description) ||
          string.IsNullOrWhiteSpace(location))
            return Result<JobResponse>.Failure("Title, description, and location are required.", ErrorType.Validation);

        if(!string.Equals(job.Title, title, StringComparison.OrdinalIgnoreCase) &&
            await unitOfWork.Jobs.NameExistsAsync(job.CompanyId, title))
            return Result<JobResponse>.Failure("A job with the same title already exists for this company.", ErrorType.Conflict);
        try
        {
            job.Update(
                title,
                description,
                location,
                request.SalaryMin,
                request.SalaryMax,
                request.EmploymentType,
                request.ExperienceLevel,
                request.ApplicationDeadline,
                currentUserId);

            unitOfWork.Jobs.Update(job);
            await unitOfWork.SaveChangesAsync();

            return Result<JobResponse>.Success(
                mapper.Map<JobResponse>(job));
        }
        catch (DomainException ex)
        {
            return Result<JobResponse>.Failure(
                ex.Message, ErrorType.Validation);
        }
    }

    private async Task<bool> CanManageCompanyAsync(
        int companyId,
        int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return false;

        if (user.Role == UserRole.Admin)
            return true;

        if (user.Role != UserRole.Recruiter)
            return false;

        var membership =
            await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(
                companyId, currentUserId);

        return membership is not null &&
            membership.Role is CompanyMemberRole.Owner
                or CompanyMemberRole.Recruiter;
    }
}
