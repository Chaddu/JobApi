namespace Application.DTOs.Request;

public record CreateJobApplicationRequest(int JobId,string ResumeUrl,string CoverLetter);
