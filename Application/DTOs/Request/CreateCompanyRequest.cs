namespace Application.DTOs.Request;

public record CreateCompanyRequest(
    string Name,
    string Description,
    string Website,
    string Location);
