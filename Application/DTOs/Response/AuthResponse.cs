namespace Application.DTOs.Response;

public record AuthResponse(
    string AccessToken,
    DateTime ExpiresAt,
    UserResponse User); 