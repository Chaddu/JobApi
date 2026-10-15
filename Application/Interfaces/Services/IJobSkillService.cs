using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IJobSkillService
{
    Task<Result<List<JobSkillResponse>>> GetByJobIdAsync(int jobId);
    Task<Result<JobSkillResponse>> AddAsync(int jobId, AddJobSkillRequest request, int currentUserId);
    Task<Result<JobSkillResponse>> UpdateAsync(int jobId, int skillId, UpdateJobSkillRequest request, int currentUserId);
    Task<Result> RemoveAsync(int jobId, int skillId, int currentUserId);
}
