using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Security.Interfaces;
using AutoMapper;
using Domain.Enums;
using System.Net.Mail;

namespace Application.Services;

public class UserService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IMapper mapper) : IUserService
{
    public async Task<Result> ChangePasswordAsync(int id, ChangePasswordRequest request, int currentUserId)
    {
       if(id != currentUserId)
            return Result.Failure("You can only change your own password.", ErrorType.Forbidden);

        var user = await unitOfWork.Users.GetByIdAsync(id);
        if (user is null)
            return Result.Failure("User not found", ErrorType.NotFound);

        if (!passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash))
            return Result.Failure("Current password is incorrect", ErrorType.Validation);

        if(string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 8)
            return Result.Failure("New password must be at least 8 characters long", ErrorType.Validation);

        var newPasswordHash = passwordHasher.HashPassword(request.NewPassword);

        user.ChangePassword(newPasswordHash, currentUserId);

        unitOfWork.Users.Update(user);
        await unitOfWork.SaveChangesAsync();
        return Result.Success("Password changed successfully");
    }

    public async Task<Result> ChangeRoleAsync(int id, ChangeUserRoleRequest request, int currentUserId)
    {
        if(!await isAdmin(currentUserId))
            return Result.Failure("You are not allowed to change user roles", ErrorType.Forbidden);

        if(!Enum.IsDefined(request.Role))
            return Result.Failure("Invalid role", ErrorType.Validation);

        var user = await unitOfWork.Users.GetByIdAsync(id);

        if(user is null)
            return Result.Failure("User not found", ErrorType.NotFound);

        user.ChangeRole(request.Role, currentUserId);

        unitOfWork.Users.Update(user);
        await unitOfWork.SaveChangesAsync();

        var response = mapper.Map<UserResponse>(user);

        return Result.Success($"User role changed to {response.Role}");

    }

    public async Task<Result> DeleteAsync(int id, int currentUserId)
    {
        if(id != currentUserId && !await isAdmin(currentUserId))
            return Result.Failure("You are not allowed to delete this user", ErrorType.Forbidden);

        var user = await unitOfWork.Users.GetByIdAsync(id);
        if(user is null)
            return Result.Failure("User not found", ErrorType.NotFound);

        unitOfWork.Users.SoftDelete(user);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("User deleted successfully");
    }

    public async Task<Result<UserResponse>> GetByIdAsync(int id, int currentUserId)
    {

        if(id != currentUserId && !await isAdmin(currentUserId))
            return Result<UserResponse>.Failure("You are not allowed to view this user", ErrorType.Forbidden);
        var user = await unitOfWork.Users.GetByIdAsync(id);

        if(user is null)
            return Result<UserResponse>.Failure("User not found", ErrorType.NotFound);

        var response = mapper.Map<UserResponse>(user);

        return Result<UserResponse>.Success(response);
    }

    public async Task<Result<UserResponse>> UpdateAsync(int id, UpdateUserRequest request, int currentUserId)
    {
        if(id != currentUserId && !await isAdmin(currentUserId))
            return Result<UserResponse>.Failure("You are not allowed to update this user", ErrorType.Forbidden);

        var user = await unitOfWork.Users.GetByIdAsync(id);

        if(user is null)
            return Result<UserResponse>.Failure("User not found", ErrorType.NotFound);

        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if(string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || !MailAddress.TryCreate(email, out _))
            return Result<UserResponse>.Failure("First name, last name and email are required", ErrorType.Validation);

        if(user.Email != email && await unitOfWork.Users.EmailExistsAsync(email))
            return Result<UserResponse>.Failure("Email is already taken", ErrorType.Conflict);

        user.UpdateProfile(
            firstName,
            lastName,
            email,
            currentUserId);

        unitOfWork.Users.Update(user);
        await unitOfWork.SaveChangesAsync();

        var response = mapper.Map<UserResponse>(user);

        return Result<UserResponse>.Success(response);
    }

    private async Task<bool> isAdmin(int userId)
    {
        var currentUser = await unitOfWork.Users.GetByIdAsync(userId);

        return currentUser?.Role == UserRole.Admin;
    }
}
