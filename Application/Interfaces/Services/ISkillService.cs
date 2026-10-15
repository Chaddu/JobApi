using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface ISkillService
{
    Task<Result<SkillResponse>> GetByIdAsync(int id);
    Task<Result<List<SkillResponse>>> GetAllAsync();
    Task<Result<SkillResponse>> CreateAsync(CreateSkillRequest request, int currentUserId);
    Task<Result<SkillResponse>> UpdateAsync(int id, UpdateSkillRequest request, int currentUserId);
    Task<Result> DeleteAsync(int id, int currentUserId);
}

