namespace Application.DTOs.Request;

public record CreateCompanyReviewRequest(int CompanyId,int Rating,string Comment);