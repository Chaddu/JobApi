namespace Application.DTOs.Response;

public record UserSkillResponse(
    int Id,
    int UserId,
    int SkillId,
    string SkillName,
    int YearsOfExperience);