using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IJobApplicationService
{
    Task<Result<JobApplicationResponse>> GetByIdAsync(int id, int currentUserId);
    Task<Result<List<JobApplicationResponse>>> GetMyApplicationsAsync(int currentUserId);
    Task<Result<List<JobApplicationResponse>>> GetByJobIdAsync(int jobId, int currentUserId);
    Task<Result<JobApplicationResponse>> ApplyAsync(CreateJobApplicationRequest request, int currentUserId);
    Task<Result<JobApplicationResponse>> UpdateAsync(int id, UpdateJobApplicationRequest request, int currentUserId);
    Task<Result> StartReviewAsync(int id, int currentUserId);
    Task<Result> ScheduleInterviewAsync(int id, int currentUserId);
    Task<Result> AcceptAsync(int id, int currentUserId);
    Task<Result> RejectAsync(int id, int currentUserId);
    Task<Result> WithdrawAsync(int id, int currentUserId);
}
