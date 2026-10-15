using Domain.Enums;

namespace Application.DTOs.Request;

public record CreateJobRequest(
    int CompanyId,
    string Title,
    string Description,
    string Location,
    decimal? SalaryMin,
    decimal? SalaryMax,
    EmploymentType EmploymentType,
    ExperienceLevel ExperienceLevel,
    DateTime? ApplicationDeadline);