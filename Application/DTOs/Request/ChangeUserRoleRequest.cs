using Domain.Enums;

namespace Application.DTOs.Request;

public record ChangeUserRoleRequest(
    UserRole Role);