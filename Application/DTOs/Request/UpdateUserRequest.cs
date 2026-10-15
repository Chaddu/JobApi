namespace Application.DTOs.Request;

public record UpdateUserRequest(
    string FirstName,
    string LastName,
    string Email);