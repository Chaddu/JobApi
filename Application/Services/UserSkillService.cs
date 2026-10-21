using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class UserSkillService(IUnitOfWork unitOfWork, IMapper mapper) : IUserSkillService
{
    public async Task<Result<UserSkillResponse>> AddAsync(AddUserSkillRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if(user is null)
            return Result<UserSkillResponse>.Failure("User not found.", ErrorType.NotFound);

        if (user.Role != UserRole.JobSeeker)
            return Result<UserSkillResponse>.Failure("Only job seekers can manage profile skills.",ErrorType.Forbidden);


        if (request.YearsOfExperience < 0)
            return Result<UserSkillResponse>.Failure("Years of experience cannot be negative.",ErrorType.Validation);

        var skill = await unitOfWork.Skills.GetByIdAsync(request.SkillId);

        if (skill is null)
            return Result<UserSkillResponse>.Failure("Skill not found.", ErrorType.NotFound);

        var existingUserSkill =await unitOfWork.UserSkills.GetByUserAndSkillAsync(currentUserId, request.SkillId);

        if (existingUserSkill is not null)
            return Result<UserSkillResponse>.Failure("You have already added this skill.",ErrorType.Conflict);

        var userSkill = new UserSkill(
            currentUserId,
            request.SkillId,
            request.YearsOfExperience);

        await unitOfWork.UserSkills.AddAsync(userSkill);
        await unitOfWork.SaveChangesAsync();

        var savedUserSkill = await unitOfWork.UserSkills.GetByUserAndSkillAsync(currentUserId, request.SkillId);

        return Result<UserSkillResponse>.Success(mapper.Map<UserSkillResponse>(savedUserSkill!));
    }

    public async Task<Result<List<UserSkillResponse>>> GetByUserIdAsync(int userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);

        if (user is null)
            return Result<List<UserSkillResponse>>.Failure("User not found", ErrorType.NotFound);

        var skills = await unitOfWork.UserSkills.GetByUserIdAsync(userId);

        return Result<List<UserSkillResponse>>.Success(mapper.Map<List<UserSkillResponse>>(skills));
    }

    public async Task<Result<List<UserSkillResponse>>> GetMySkillsAsync(int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<List<UserSkillResponse>>.Failure("User not found.", ErrorType.NotFound);

        var skills = await unitOfWork.UserSkills.GetByUserIdAsync(currentUserId);

        return Result<List<UserSkillResponse>>.Success(mapper.Map<List<UserSkillResponse>>(skills));
    }

    public async Task<Result> RemoveAsync(int skillId, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result.Failure("User not found.", ErrorType.NotFound);

        if (user.Role != UserRole.JobSeeker)
            return Result.Failure("Only job seekers can manage profile skills.",ErrorType.Forbidden);

        var userSkill =await unitOfWork.UserSkills.GetByUserAndSkillAsync(currentUserId, skillId);

        if (userSkill is null)
            return Result.Failure("Your profile does not contain this skill.",ErrorType.NotFound);

        unitOfWork.UserSkills.Delete(userSkill);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Skill removed from your profile.");
    }

    public async Task<Result<UserSkillResponse>> UpdateAsync(int skillId, UpdateUserSkillRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<UserSkillResponse>.Failure("User not found.", ErrorType.NotFound);

        if (user.Role != UserRole.JobSeeker)
            return Result<UserSkillResponse>.Failure("Only job seekers can manage profile skills.",ErrorType.Forbidden);

        var userSkill = await unitOfWork.UserSkills.GetByUserAndSkillAsync(currentUserId, skillId);

        if (userSkill is null)
            return Result<UserSkillResponse>.Failure("Your profile does not contain this skill.",ErrorType.NotFound);

        if (request.YearsOfExperience < 0)
            return Result<UserSkillResponse>.Failure("Years of experience cannot be negative.",ErrorType.Validation);

        userSkill.UpdateExperience(request.YearsOfExperience, currentUserId);

        unitOfWork.UserSkills.Update(userSkill);
        await unitOfWork.SaveChangesAsync();

        return Result<UserSkillResponse>.Success(mapper.Map<UserSkillResponse>(userSkill));
    }
}
