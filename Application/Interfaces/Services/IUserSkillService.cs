using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IUserSkillService
{
    Task<Result<List<UserSkillResponse>>> GetMySkillsAsync(int currentUserId);
    Task<Result<List<UserSkillResponse>>> GetByUserIdAsync(int userId);
    Task<Result<UserSkillResponse>> AddAsync(AddUserSkillRequest request, int currentUserId);
    Task<Result<UserSkillResponse>> UpdateAsync(int skillId, UpdateUserSkillRequest request, int currentUserId);
    Task<Result> RemoveAsync(int skillId, int currentUserId);
}
