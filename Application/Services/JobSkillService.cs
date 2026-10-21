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

public class JobSkillService(IUnitOfWork unitOfWork, IMapper mapper) : IJobSkillService
{
    public async Task<Result<JobSkillResponse>> AddAsync(int jobId, AddJobSkillRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<JobSkillResponse>.Failure("User not found.", ErrorType.NotFound);

        var job = await unitOfWork.Jobs.GetByIdAsync(jobId);

        if (job is null)
            return Result<JobSkillResponse>.Failure("Job not found.", ErrorType.NotFound);

        if (job.Status == JobStatus.Closed)
            return Result<JobSkillResponse>.Failure("Skills cannot be changed for a closed job.",ErrorType.Conflict);

        if(user.Role != UserRole.Admin)
        {
            var membership = await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(job.CompanyId, currentUserId);

            if (membership is null)
                return Result<JobSkillResponse>.Failure("You are not allowed to manage this job's skills.",ErrorType.Forbidden);
        }

        var skill = await unitOfWork.Skills.GetByIdAsync(request.SkillId);

        if (skill is null)
            return Result<JobSkillResponse>.Failure("Skill not found.", ErrorType.NotFound);

        var existingJobSkill = await unitOfWork.JobSkills.GetByJobAndSkillAsync(jobId, request.SkillId);

        if (existingJobSkill is not null)
            return Result<JobSkillResponse>.Failure("This skill has already been added to the job.",ErrorType.Conflict);


        var jobSkill = new JobSkill(jobId, request.SkillId, request.IsRequired);

        await unitOfWork.JobSkills.AddAsync(jobSkill);
        await unitOfWork.SaveChangesAsync();

        var savedJobSkill = await unitOfWork.JobSkills.GetByJobAndSkillAsync(jobId, request.SkillId);

        return Result<JobSkillResponse>.Success(mapper.Map<JobSkillResponse>(savedJobSkill!));
    }

    public async Task<Result<List<JobSkillResponse>>> GetByJobIdAsync(int jobId)
    {
        var job = await unitOfWork.Jobs.GetByIdAsync(jobId);

        if (job is null)
            return Result<List<JobSkillResponse>>.Failure("Job not found", ErrorType.NotFound);

        var jobSkills =await unitOfWork.JobSkills.GetByJobIdAsync(jobId);

        return Result<List<JobSkillResponse>>.Success(mapper.Map<List<JobSkillResponse>>(jobSkills));
    }

    public async Task<Result> RemoveAsync(int jobId, int skillId, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result.Failure("User not found.", ErrorType.NotFound);

        var job = await unitOfWork.Jobs.GetByIdAsync(jobId);

        if (job is null)
            return Result.Failure("Job not found.", ErrorType.NotFound);

        if (job.Status == JobStatus.Closed)
            return Result.Failure("Skills cannot be changed for a closed job.", ErrorType.Conflict);

        if (user.Role != UserRole.Admin)
        {
            var membership = await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(job.CompanyId, currentUserId);

            if (membership is null)
                return Result.Failure("You are not allowed to manage this job's skills.",ErrorType.Forbidden);
        }

        var jobSkill = await unitOfWork.JobSkills.GetByJobAndSkillAsync(jobId, skillId);

        if (jobSkill is null)
            return Result.Failure("This skill is not associated with the job.",ErrorType.NotFound);

        unitOfWork.JobSkills.Delete(jobSkill);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Skill removed from the job.");
    }

    public async Task<Result<JobSkillResponse>> UpdateAsync(int jobId, int skillId, UpdateJobSkillRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<JobSkillResponse>.Failure("User not found.", ErrorType.NotFound);

        var job = await unitOfWork.Jobs.GetByIdAsync(jobId);

        if (job is null)
            return Result<JobSkillResponse>.Failure("Job not found.", ErrorType.NotFound);

        if (job.Status == JobStatus.Closed)
            return Result<JobSkillResponse>.Failure("Skills cannot be changed for a closed job.",ErrorType.Conflict);

        if (user.Role != UserRole.Admin)
        {
            var membership = await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(job.CompanyId, currentUserId);

            if (membership is null)
                return Result<JobSkillResponse>.Failure("You are not allowed to manage this job's skills.",ErrorType.Forbidden);
        }

        var jobSkill = await unitOfWork.JobSkills.GetByJobAndSkillAsync(jobId, skillId);

        if (jobSkill is null)
            return Result<JobSkillResponse>.Failure("This skill is not associated with the job.", ErrorType.NotFound);
        try
        {
            jobSkill.SetRequired(request.IsRequired, currentUserId);

            unitOfWork.JobSkills.Update(jobSkill);
            await unitOfWork.SaveChangesAsync();

            return Result<JobSkillResponse>.Success(mapper.Map<JobSkillResponse>(jobSkill));
        }
        catch(DomainException ex)
        {
            return Result<JobSkillResponse>.Failure(ex.Message, ErrorType.Validation);
        }
    }
}
