namespace Application.DTOs.Response;

public record CompanyResponse(
    int Id,
    string Name,
    string Description,
    string Website,
    string Location,
    DateTime CreatedAt);