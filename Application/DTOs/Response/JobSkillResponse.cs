namespace Application.DTOs.Response;


public record JobSkillResponse(
    int Id,
    int JobId,
    int SkillId,
    string SkillName,
    bool IsRequired);
