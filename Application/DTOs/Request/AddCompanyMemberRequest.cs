using Domain.Enums;

namespace Application.DTOs.Request;

public record AddCompanyMemberRequest(
    int UserId,
    CompanyMemberRole Role);