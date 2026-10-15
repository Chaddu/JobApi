using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;

public interface ICompanyReviewService
{
    Task<Result<CompanyReviewResponse>> GetByIdAsync(int id);
    Task<Result<List<CompanyReviewResponse>>> GetByCompanyIdAsync(int companyId);
    Task<Result<CompanyReviewResponse>> CreateAsync(CreateCompanyReviewRequest request, int currentUserId);
    Task<Result<CompanyReviewResponse>> UpdateAsync(int id, UpdateCompanyReviewRequest request, int currentUserId);
    Task<Result> DeleteAsync(int id, int currentUserId);
}
