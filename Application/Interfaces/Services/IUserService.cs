using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface IUserService
{
    Task<Result<UserResponse>> GetByIdAsync(int id);
    Task<Result<UserResponse>> UpdateAsync(int id, UpdateUserRequest request, int currentUserId);
    Task<Result> ChangeRoleAsync(int id, ChangeUserRoleRequest request, int currentUserId);
    Task<Result> ChangePasswordAsync(int id, ChangePasswordRequest request, int currentUserId);
    Task<Result> DeleteAsync(int id, int currentUserId);
}
