using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class SavedJobService(IUnitOfWork unitOfWork, IMapper mapper) : ISavedJobService
{
    public async Task<Result<List<SavedJobResponse>>> GetMySavedJobsAsync(int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<List<SavedJobResponse>>.Failure("User not found", ErrorType.NotFound);

        if(user.Role != UserRole.JobSeeker)
            return Result<List<SavedJobResponse>>.Failure("Only job seekers can view saved jobs.",ErrorType.Forbidden);

        var savedJobs = await unitOfWork.SavedJobs.GetByUserIdAsync(currentUserId);

        var response = mapper.Map<List<SavedJobResponse>>(savedJobs);

        return Result<List<SavedJobResponse>>.Success(response);
    }

    public async Task<Result> RemoveAsync(int jobId, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result.Failure("User not found.", ErrorType.NotFound);

        if (user.Role != UserRole.JobSeeker)
            return Result.Failure("Only job seekers can remove saved jobs.",ErrorType.Forbidden);

        var savedJob = await unitOfWork.SavedJobs.GetByUserAndJobAsync(currentUserId, jobId);

        if (savedJob is null)
            return Result.Failure("Saved job not found.", ErrorType.NotFound);

        unitOfWork.SavedJobs.Delete(savedJob);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Saved job removed successfully.");

    }

    public async Task<Result<SavedJobResponse>> SaveAsync(SaveJobRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<SavedJobResponse>.Failure("User not found.", ErrorType.NotFound);

        if (user.Role != UserRole.JobSeeker)
            return Result<SavedJobResponse>.Failure("Only job seekers can save jobs.", ErrorType.Forbidden);

        var job = await unitOfWork.Jobs.GetByIdAsync(request.JobId);
        if (job is null || job.Status != JobStatus.Open)
            return Result<SavedJobResponse>.Failure("Job not found or is no longer open.",ErrorType.NotFound);

        var existingSavedJob =
            await unitOfWork.SavedJobs.GetByUserAndJobAsync(
                currentUserId, request.JobId);

        if (existingSavedJob is not null)
            return Result<SavedJobResponse>.Failure("You have already saved this job.",ErrorType.Conflict);

        var savedJob = new SavedJob(currentUserId, request.JobId);

        await unitOfWork.SavedJobs.AddAsync(savedJob);
        await unitOfWork.SaveChangesAsync();

        return Result<SavedJobResponse>.Success(mapper.Map<SavedJobResponse>(savedJob));
    }
}
