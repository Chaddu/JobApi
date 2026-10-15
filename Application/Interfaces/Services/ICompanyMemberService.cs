using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface ICompanyMemberService
{
    Task<Result<List<CompanyMemberResponse>>> GetByCompanyIdAsync(int companyId, int currentUserId);
    Task<Result<List<CompanyMemberResponse>>> GetByUserIdAsync(int userId, int currentUserId);
    Task<Result<CompanyMemberResponse>> AddAsync(int companyId, AddCompanyMemberRequest request, int currentUserId);
    Task<Result> ChangeRoleAsync(int companyId, int userId, ChangeCompanyMemberRoleRequest request, int currentUserId);
    Task<Result> RemoveAsync(int companyId, int userId, int currentUserId);
}
