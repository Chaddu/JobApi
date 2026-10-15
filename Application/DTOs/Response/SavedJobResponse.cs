namespace Application.DTOs.Response;

public record SavedJobResponse(
    int Id,
    int UserId,
    int JobId,
    DateTime CreatedAt);