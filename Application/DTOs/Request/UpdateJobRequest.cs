using Domain.Enums;

namespace Application.DTOs.Request;

public record UpdateJobRequest(
    string Title,
    string Description,
    string Location,
    decimal? SalaryMin,
    decimal? SalaryMax,
    EmploymentType EmploymentType,
    ExperienceLevel ExperienceLevel,
    DateTime? ApplicationDeadline); 