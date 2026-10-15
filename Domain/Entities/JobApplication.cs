using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;
public class JobApplication : BaseEntity<int>
{
    public int JobId { get; private set; }
    public int ApplicantId { get; private set; }

    public string ResumeUrl { get; private set; } = string.Empty;
    public string CoverLetter { get; private set; } = string.Empty;

    public ApplicationStatus Status { get; private set; }
    public DateTime AppliedAt { get; private set; }

    public Job Job { get; private set; } = null!;
    public User Applicant { get; private set; } = null!;

    private JobApplication()
    {
    }

    public JobApplication(
        int jobId,
        int applicantId,
        string resumeUrl,
        string coverLetter)
    {
        JobId = jobId;
        ApplicantId = applicantId;
        ResumeUrl = resumeUrl;
        CoverLetter = coverLetter;
        Status = ApplicationStatus.Pending;
        AppliedAt = DateTime.UtcNow;
    }

    public void Update(
        string resumeUrl,
        string coverLetter,
        int? updatedById = null)
    {
        if (Status is ApplicationStatus.Accepted
            or ApplicationStatus.Rejected
            or ApplicationStatus.Withdrawn)
        {
            throw new DomainException(
                "A finalized application cannot be updated.");
        }

        ResumeUrl = resumeUrl;
        CoverLetter = coverLetter;

        MarkAsUpdated(updatedById);
    }

    public void StartReview(int? updatedById = null)
    {
        EnsureStatus(ApplicationStatus.Pending);

        Status = ApplicationStatus.Reviewing;

        MarkAsUpdated(updatedById);
    }

    public void ScheduleInterview(int? updatedById = null)
    {
        EnsureStatus(ApplicationStatus.Reviewing);

        Status = ApplicationStatus.Interview;

        MarkAsUpdated(updatedById);
    }

    public void Accept(int? updatedById = null)
    {
        EnsureStatus(ApplicationStatus.Interview);

        Status = ApplicationStatus.Accepted;

        MarkAsUpdated(updatedById);
    }

    public void Reject(int? updatedById = null)
    {
        if (Status is not (
            ApplicationStatus.Pending
            or ApplicationStatus.Reviewing
            or ApplicationStatus.Interview))
        {
            throw new DomainException("Only pending, reviewing, or interview applications can be rejected.");
        }

        Status = ApplicationStatus.Rejected;

        MarkAsUpdated(updatedById);
    }

    public void Withdraw(int? updatedById = null)
    {
        if (Status is not (
            ApplicationStatus.Pending
            or ApplicationStatus.Reviewing))
        {
            throw new DomainException("Only pending or reviewing applications can be withdrawn.");
        }

        Status = ApplicationStatus.Withdrawn;

        MarkAsUpdated(updatedById);
    }

    private void EnsureStatus(ApplicationStatus expectedStatus)
    {
        if (Status != expectedStatus)
        {
            throw new DomainException($"Application must be {expectedStatus}.");
        }
    }
}
