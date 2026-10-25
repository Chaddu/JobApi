using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using AutoMapper.Execution;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CompanyMemberService(IUnitOfWork unitOfWork, IMapper mapper) : ICompanyMemberService
{
    public async Task<Result<CompanyMemberResponse>> AddAsync(int companyId, AddCompanyMemberRequest request, int currentUserId)
    {
        var permission = await CheckOwnerOrAdminAsync(companyId, currentUserId);

        if (permission is not null)
            return Result<CompanyMemberResponse>.Failure(permission.Error, permission.ErrorType);

        if (!Enum.IsDefined(request.Role))
            return Result<CompanyMemberResponse>.Failure("Invalid company member role.", ErrorType.Validation);

        var user = await unitOfWork.Users.GetByIdAsync(request.UserId);

        if (user is null)
            return Result<CompanyMemberResponse>.Failure("User not found.", ErrorType.NotFound);

        var existingMembership = await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(companyId, request.UserId);

        if (existingMembership is not null)
            return Result<CompanyMemberResponse>.Failure("This user is already a member of the company.",ErrorType.Conflict);

        var currentUser = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (request.Role == CompanyMemberRole.Owner &&
            currentUser!.Role != UserRole.Admin)
            return Result<CompanyMemberResponse>.Failure("Only admins can assign the Owner role.", ErrorType.Forbidden);

        var membership = new CompanyMember(
            companyId,
            request.UserId,
            request.Role);

        await unitOfWork.CompanyMembers.AddAsync(membership);
        await unitOfWork.SaveChangesAsync();

        return Result<CompanyMemberResponse>.Success(mapper.Map<CompanyMemberResponse>(membership));
    }

    public async Task<Result> ChangeRoleAsync(int companyId, int userId, ChangeCompanyMemberRoleRequest request, int currentUserId)
    {
        var permission = await CheckOwnerOrAdminAsync(companyId, currentUserId);

        if (permission is not null)
            return permission;

        if (!Enum.IsDefined(request.Role))
            return Result.Failure("Invalid company member role.", ErrorType.Validation);

        var membership = await unitOfWork.CompanyMembers
            .GetByCompanyAndUserAsync(companyId, userId);

        if (membership is null)
            return Result.Failure("Company membership not found.", ErrorType.NotFound);

        var currentUser = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (request.Role == CompanyMemberRole.Owner &&
            currentUser!.Role != UserRole.Admin)
        {
            return Result.Failure("Only admins can assign the Owner role.", ErrorType.Forbidden);
        }

        if (membership.Role == CompanyMemberRole.Owner &&
            request.Role != CompanyMemberRole.Owner)
        {
            var members = await unitOfWork.CompanyMembers.GetByCompanyIdAsync(companyId);
            var ownerCount = members.Count(member =>
                member.Role == CompanyMemberRole.Owner);

            if (ownerCount <= 1)
                return Result.Failure("The company must have at least one Owner.", ErrorType.Conflict);
        }

        try
        {
            membership.ChangeRole(request.Role, currentUserId);
        }
        catch (DomainException exception)
        {
            return Result.Failure(exception.Message, ErrorType.Validation);
        }

        unitOfWork.CompanyMembers.Update(membership);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Company member role updated.");
    }

    public async Task<Result<List<CompanyMemberResponse>>> GetByCompanyIdAsync(int companyId, int currentUserId)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(companyId);

        if (company is null)
            return Result<List<CompanyMemberResponse>>.Failure("Company not found", ErrorType.NotFound);

        var currentUser = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (currentUser is null)
            return Result<List<CompanyMemberResponse>>.Failure("User not found", ErrorType.NotFound);

        var isAdmin = currentUser.Role == UserRole.Admin;
        var membership = await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(companyId, currentUserId);

        if (!isAdmin && membership is null)
            return Result<List<CompanyMemberResponse>>.Failure("You are not a member of this company.", ErrorType.Forbidden);

        var members = await unitOfWork.CompanyMembers.GetByCompanyIdAsync(companyId);

        return Result<List<CompanyMemberResponse>>.Success(mapper.Map<List<CompanyMemberResponse>>(members));
    }

    public async Task<Result<List<CompanyMemberResponse>>> GetByUserIdAsync(int userId, int currentUserId)
    {
        var currentUser = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (currentUser is null)
            return Result<List<CompanyMemberResponse>>.Failure("User not found", ErrorType.NotFound);

        if (userId != currentUserId && currentUser.Role != UserRole.Admin)
            return Result<List<CompanyMemberResponse>>.Failure("You are not allowed to view this user's memberships.", ErrorType.Forbidden);

        var user = await unitOfWork.Users.GetByIdAsync(userId);

        if (user is null)
            return Result<List<CompanyMemberResponse>>.Failure("User not found.", ErrorType.NotFound);

        var memberships = await unitOfWork.CompanyMembers.GetByUserIdAsync(userId);

        return Result<List<CompanyMemberResponse>>.Success(mapper.Map<List<CompanyMemberResponse>>(memberships));

    }

    public async Task<Result> RemoveAsync(int companyId, int userId, int currentUserId)
    {
        var permission = await CheckOwnerOrAdminAsync(companyId, currentUserId);

        if (permission is not null)
            return permission;

        var membership = await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(companyId, userId);

        if (membership is null)
            return Result.Failure("Company membership not found.", ErrorType.NotFound);

        if (membership.Role == CompanyMemberRole.Owner)
        {
            var members = await unitOfWork.CompanyMembers.GetByCompanyIdAsync(companyId);
            var ownerCount = members.Count(member => member.Role == CompanyMemberRole.Owner);

            if (ownerCount <= 1)
                return Result.Failure("The last Owner cannot be removed.",ErrorType.Conflict);
        }

        unitOfWork.CompanyMembers.SoftDelete(membership, currentUserId);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Company member removed.");
    }

    private async Task<Result?> CheckOwnerOrAdminAsync(int companyId, int currentUserId)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(companyId);

        if (company is null)
            return Result.Failure("Company not found.", ErrorType.NotFound);

        var currentUser = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (currentUser is null)
            return Result.Failure("User not found.", ErrorType.NotFound);

        if (currentUser.Role == UserRole.Admin)
            return null;

        var membership = await unitOfWork.CompanyMembers
            .GetByCompanyAndUserAsync(companyId, currentUserId);

        if (membership?.Role != CompanyMemberRole.Owner)
            return Result.Failure("Only company Owners or admins can manage members.", ErrorType.Forbidden);

        return null;
    }
}
