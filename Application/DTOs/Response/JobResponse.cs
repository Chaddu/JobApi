using Domain.Enums;

namespace Application.DTOs.Response;

public record JobResponse(
    int Id,
    int CompanyId,
    int CreatedById,
    string Title,
    string Description,
    string Location,
    decimal? SalaryMin,
    decimal? SalaryMax,
    EmploymentType EmploymentType,
    ExperienceLevel ExperienceLevel,
    JobStatus Status,
    DateTime? ApplicationDeadline,
    DateTime CreatedAt,
    byte[] RowVersion);