using Domain.Enums;

namespace Application.DTOs.Request;

public record ChangeCompanyMemberRoleRequest(
    CompanyMemberRole Role);