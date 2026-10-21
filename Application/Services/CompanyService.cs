using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class CompanyService(IUnitOfWork unitOfWork, IMapper mapper) : ICompanyService
{
    public async Task<Result<CompanyResponse>> CreateAsync(CreateCompanyRequest request, int currentUserId)
    {
        var name = request.Name?.Trim() ?? string.Empty;
        var description = request.Description?.Trim() ?? string.Empty;
        var website = request.Website?.Trim() ?? string.Empty;
        var location = request.Location?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name) ||
        string.IsNullOrWhiteSpace(description) ||
        string.IsNullOrWhiteSpace(website) ||
        string.IsNullOrWhiteSpace(location))
            return Result<CompanyResponse>.Failure("All fields are required", ErrorType.Validation);
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return Result<CompanyResponse>.Failure("User not found.",ErrorType.NotFound);

        if (user.Role != UserRole.Recruiter &&user.Role != UserRole.Admin)
            return Result<CompanyResponse>.Failure("Only recruiters and administrators can create companies.",ErrorType.Forbidden);

        var company = new Company(
            name,
            description,
            website,
            location);
        await unitOfWork.ExecuteInTransactionAsync(async() =>
        {
            await unitOfWork.Companies.AddAsync(company);

            await unitOfWork.SaveChangesAsync();

            var companyMember = new CompanyMember(
                company.Id,
                currentUserId,
                CompanyMemberRole.Owner);

            await unitOfWork.CompanyMembers.AddAsync(companyMember);

            await unitOfWork.SaveChangesAsync();
        });

        var response = mapper.Map<CompanyResponse>(company);

        return Result<CompanyResponse>.Success(response);
    }

    public async Task<Result> DeleteAsync(int id, int currentUserId)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(id);

        if(company is null)
            return Result.Failure("Company not found", ErrorType.NotFound);

        var currentUser = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if(currentUser is null)
            return Result.Failure("User not found", ErrorType.NotFound);
        
        if(currentUser.Role != UserRole.Admin)
        {
            var membership = await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(id, currentUserId);

            if(membership is null || membership.Role != CompanyMemberRole.Owner)
                return Result.Failure("You are not allowed to delete this company.", ErrorType.Forbidden);
        }
        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var jobs = await unitOfWork.Jobs.GetByCompanyIdAsync(id);

            var members = await unitOfWork.CompanyMembers.GetByCompanyIdAsync(id);

            foreach (var job in jobs)
            {
                unitOfWork.Jobs.SoftDelete(job, currentUserId);
            }

            foreach (var member in members)
            {
                unitOfWork.CompanyMembers.SoftDelete(member, currentUserId);
            }

            unitOfWork.Companies.SoftDelete(company, currentUserId);

            await unitOfWork.SaveChangesAsync();
        });

        return Result.Success("Company deleted successfully");
    }

    public async Task<Result<List<CompanyResponse>>> GetAllAsync()
    {
        var companies = await unitOfWork.Companies.GetAllAsync();

        var response = mapper.Map<List<CompanyResponse>>(companies);

        return Result<List<CompanyResponse>>.Success(response);
    }

    public async Task<Result<CompanyResponse>> GetByIdAsync(int id)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(id);
        if (company is null)
            return Result<CompanyResponse>.Failure("Company not found", ErrorType.NotFound);

        var response = mapper.Map<CompanyResponse>(company);
        return Result<CompanyResponse>.Success(response);
    }

    public async Task<Result<CompanyResponse>> UpdateAsync(int id, UpdateCompanyRequest request, int currentUserId)
    {
        var company = await unitOfWork.Companies.GetByIdAsync(id);

        if(company is null)
            return Result<CompanyResponse>.Failure("Company not found", ErrorType.NotFound);

        var currentUser = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if(currentUser is null)
            return Result<CompanyResponse>.Failure("User not found", ErrorType.NotFound);

        if(currentUser.Role != UserRole.Admin)
        {
            var membership = await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(id, currentUserId);

            if (membership is null || (membership.Role != CompanyMemberRole.Owner && membership.Role != CompanyMemberRole.Recruiter))
                return Result<CompanyResponse>.Failure("You are not allowed to update this company.", ErrorType.Forbidden);
        }
        var name = request.Name.Trim();
        var description = request.Description.Trim();
        var website = request.Website.Trim();
        var location = request.Location.Trim();

        if (string.IsNullOrWhiteSpace(name) ||
       string.IsNullOrWhiteSpace(description) ||
       string.IsNullOrWhiteSpace(website) ||
       string.IsNullOrWhiteSpace(location))
        {
            return Result<CompanyResponse>.Failure("All company fields are required.",ErrorType.Validation);
        }

        company.Update(name, description, website, location, currentUserId);

        unitOfWork.Companies.Update(company);
        await unitOfWork.SaveChangesAsync();

        var response = mapper.Map<CompanyResponse>(company);

        return Result<CompanyResponse>.Success(response);

    }
}
