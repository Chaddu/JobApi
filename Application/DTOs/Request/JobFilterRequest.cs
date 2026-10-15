using Domain.Enums;

namespace Application.DTOs.Request;

public record JobFilterRequest(
    int Page = 1,
    int PageSize = 10,
    string? Search = null,
    string? Location = null,
    EmploymentType? EmploymentType = null,
    ExperienceLevel? ExperienceLevel = null,
    decimal? MinSalary = null,
    decimal? MaxSalary = null,
    string? Skill = null,
    string SortBy = "CreatedAt",
    string SortDirection = "Desc");