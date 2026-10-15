namespace Application.DTOs.Response;

public record CompanyReviewResponse(
    int Id,
    int CompanyId,
    int UserId,
    string UserName,
    int Rating,
    string Comment,
    DateTime CreatedAt,
    DateTime? UpdatedAt);