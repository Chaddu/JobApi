namespace Application.DTOs.Request;

public record UpdateCompanyRequest(
    string Name,
    string Description,
    string Website,
    string Location);