using Domain.Enums;

namespace Application.DTOs.Response;

public record JobApplicationResponse(
    int Id,
    int JobId,
    int ApplicantId,
    string ResumeUrl,
    string CoverLetter,
    ApplicationStatus Status,
    DateTime AppliedAt,
    DateTime? UpdatedAt);