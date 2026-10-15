namespace Application.DTOs.Request;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password);