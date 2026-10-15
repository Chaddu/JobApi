using Domain.Enums;

namespace Application.DTOs.Response;

public record CompanyMemberResponse(
    int Id,
    int CompanyId,
    int UserId,
    CompanyMemberRole Role,
    DateTime JoinedAt);