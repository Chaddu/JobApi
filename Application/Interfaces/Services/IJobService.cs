using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IJobService
{
    Task<Result<JobResponse>> GetByIdAsync(int id);
    Task<Result<PagedResult<JobResponse>>> SearchAsync(JobFilterRequest request);
    Task<Result<List<JobResponse>>> GetByCompanyIdAsync(int companyId, int currentUserId);
    Task<Result<JobResponse>> CreateAsync(CreateJobRequest request, int currentUserId);
    Task<Result<JobResponse>> UpdateAsync(int id, UpdateJobRequest request, int currentUserId);
    Task<Result> PublishAsync(int id, int currentUserId);
    Task<Result> CloseAsync(int id, int currentUserId);
}
