using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces.Services;
public interface ICompanyService
{
    Task<Result<CompanyResponse>> GetByIdAsync(int id);
    Task<Result<List<CompanyResponse>>> GetAllAsync();
    Task<Result<CompanyResponse>> CreateAsync(CreateCompanyRequest request, int currentUserId);
    Task<Result<CompanyResponse>> UpdateAsync(int id, UpdateCompanyRequest request, int currentUserId);
    Task<Result> DeleteAsync(int id, int currentUserId);
}
