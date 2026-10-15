using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface ISavedJobService
{
    Task<Result<List<SavedJobResponse>>> GetMySavedJobsAsync(int currentUserId);
    Task<Result<SavedJobResponse>> SaveAsync(SaveJobRequest request, int currentUserId);
    Task<Result> RemoveAsync(int jobId, int currentUserId);
}
