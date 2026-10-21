using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class SkillService(IUnitOfWork unitOfWork, IMapper mapper) : ISkillService
{
    public async Task<Result<SkillResponse>> CreateAsync(CreateSkillRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<SkillResponse>.Failure("User not found", ErrorType.NotFound);

        if (user.Role != UserRole.Admin)
            return Result<SkillResponse>.Failure("Only administrators can manage skills.", ErrorType.Forbidden);

        var name = request.Name?.Trim() ?? string.Empty;
        var description = request.Description?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            return Result<SkillResponse>.Failure("Skill name is required.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(description))
            return Result<SkillResponse>.Failure("Skill description is required.", ErrorType.Validation);

        var existingSkill = await unitOfWork.Skills.GetByNameAsync(name);

        if(existingSkill is not null)
            return Result<SkillResponse>.Failure("A skill with this name already exists.",ErrorType.Conflict);

        var skill = new Skill(name, description);

        await unitOfWork.Skills.AddAsync(skill);
        await unitOfWork.SaveChangesAsync();

        return Result<SkillResponse>.Success(mapper.Map<SkillResponse>(skill));

    }

    public async Task<Result> DeleteAsync(int id, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<SkillResponse>.Failure("User not found", ErrorType.NotFound);

        if (user.Role != UserRole.Admin)
            return Result<SkillResponse>.Failure("Only administrators can manage skills.", ErrorType.Forbidden);

        var skill = await unitOfWork.Skills.GetByIdAsync(id);

        if (skill is null)
            return Result.Failure("Skill not found.", ErrorType.NotFound);

        unitOfWork.Skills.SoftDelete(skill, currentUserId);

        await unitOfWork.SaveChangesAsync();

        return Result.Success("Skill deleted successfully.");
    }

    public async Task<Result<List<SkillResponse>>> GetAllAsync()
    {
        var skills = await unitOfWork.Skills.GetAllAsync();

        return Result<List<SkillResponse>>.Success(mapper.Map<List<SkillResponse>>(skills));
    }

    public async Task<Result<SkillResponse>> GetByIdAsync(int id)
    {
        var skill = await unitOfWork.Skills.GetByIdAsync(id);

        if (skill is null)
            return Result<SkillResponse>.Failure("Skill not found.", ErrorType.NotFound);

        return Result<SkillResponse>.Success(mapper.Map<SkillResponse>(skill));
    }

    public async Task<Result<SkillResponse>> UpdateAsync(int id, UpdateSkillRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<SkillResponse>.Failure("User not found", ErrorType.NotFound);

        if (user.Role != UserRole.Admin)
            return Result<SkillResponse>.Failure("Only administrators can manage skills.", ErrorType.Forbidden);

        var skill = await unitOfWork.Skills.GetByIdAsync(id);

        if (skill is null)
            return Result<SkillResponse>.Failure("Skill not found", ErrorType.NotFound);

        var name = request.Name?.Trim() ?? string.Empty;
        var description = request.Description?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name))
            return Result<SkillResponse>.Failure("Skill name is required.", ErrorType.Validation);

        if (string.IsNullOrWhiteSpace(description))
            return Result<SkillResponse>.Failure("Skill description is required.", ErrorType.Validation);

        var existingSkill = await unitOfWork.Skills.GetByNameAsync(name);

        if(existingSkill is not null &&  existingSkill.Id != id)
            return Result<SkillResponse>.Failure("A skill with this name already exists.",ErrorType.Conflict);

        skill.Rename(name, description, currentUserId);

        unitOfWork.Skills.Update(skill);
        await unitOfWork.SaveChangesAsync();

        return Result<SkillResponse>.Success(mapper.Map<SkillResponse>(skill));
    }

}
