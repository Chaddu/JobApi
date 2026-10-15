namespace Application.DTOs.Response;
public record UserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Role,
    DateTime CreatedAt);