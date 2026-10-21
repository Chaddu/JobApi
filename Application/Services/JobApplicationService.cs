using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Services;
using AutoMapper;
using AutoMapper.Execution;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.Services;

public class JobApplicationService(IUnitOfWork unitOfWork, IMapper mapper) : IJobApplicationService
{

    public async Task<Result<JobApplicationResponse>> ApplyAsync(CreateJobApplicationRequest request, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if(user is null)
            return Result<JobApplicationResponse>.Failure("User not found.", ErrorType.NotFound);

        if(user.Role != UserRole.JobSeeker)
            return Result<JobApplicationResponse>.Failure("Only job seekers can apply for jobs.", ErrorType.Forbidden);

        var resumeUrl = request.ResumeUrl;
        var coverLetter = request.CoverLetter;

        if(string.IsNullOrWhiteSpace(resumeUrl)
            || string.IsNullOrWhiteSpace(coverLetter))
            return Result<JobApplicationResponse>.Failure("Resume URL and cover letter are required.", ErrorType.Validation);

        var job = await unitOfWork.Jobs.GetByIdAsync(request.JobId);

        if(job is null)
            return Result<JobApplicationResponse>.Failure("Job not found.", ErrorType.NotFound);

        if(job.Status != JobStatus.Open)
            return Result<JobApplicationResponse>.Failure("Job is not open for applications.", ErrorType.Validation);

        if (job.ApplicationDeadline.HasValue && job.ApplicationDeadline.Value <= DateTime.UtcNow)
            return Result<JobApplicationResponse>.Failure("The application deadline has passed.",ErrorType.Validation);

        if(await unitOfWork.JobApplications.HasAppliedAsync(request.JobId, currentUserId))
            return Result<JobApplicationResponse>.Failure("You have already applied for this job.", ErrorType.Conflict);

        var application = new JobApplication(job.Id, currentUserId, resumeUrl, coverLetter);

        await unitOfWork.JobApplications.AddAsync(application);

        var memebers = await unitOfWork.CompanyMembers.GetByCompanyIdAsync(job.CompanyId);

        foreach(var m in memebers)
        {
            if(m.UserId == currentUserId)
                continue;


            await CreateNotificationAsync(m.UserId,"New job application",$"Someone applied for the job '{job.Title}'.",NotificationType.ApplicationSubmitted);
        }
        await unitOfWork.SaveChangesAsync();



        var response = mapper.Map<JobApplicationResponse>(application);
        return Result<JobApplicationResponse>.Success(response);
    }

    public async Task<Result<JobApplicationResponse>> GetByIdAsync(int id, int currentUserId)
    {
        var application = await unitOfWork.JobApplications.GetByIdAsync(id);

        if (application is null)
            return Result<JobApplicationResponse>.Failure("Job application not found.", ErrorType.NotFound);

        var job = await unitOfWork.Jobs.GetByIdAsync(application.JobId);

        if(job is null)
            return Result<JobApplicationResponse>.Failure("Job not found.", ErrorType.NotFound);

        if (application.ApplicantId != currentUserId && ! await CanManageApplicationsAsync(job.CompanyId, currentUserId))
            return Result<JobApplicationResponse>.Failure("You are not allowwed to view this job application.", ErrorType.Forbidden);

        var response = mapper.Map<JobApplicationResponse>(application);

        return Result<JobApplicationResponse>.Success(response);

    }

    public async Task<Result<List<JobApplicationResponse>>> GetByJobIdAsync(int jobId, int currentUserId)
    {
        var job = await unitOfWork.Jobs.GetByIdAsync(jobId);

        if(job is null)
            return Result<List<JobApplicationResponse>>.Failure("Job not found.", ErrorType.NotFound);

        if(!await CanManageApplicationsAsync(job.CompanyId, currentUserId))
            return Result<List<JobApplicationResponse>>.Failure("You are not allowwed to view this job application.", ErrorType.Forbidden);

        var applications = await unitOfWork.JobApplications.GetByJobIdAsync(jobId);

        var response = mapper.Map<List<JobApplicationResponse>>(applications);

        return Result<List<JobApplicationResponse>>.Success(response);
    }

    public async Task<Result<List<JobApplicationResponse>>> GetMyApplicationsAsync(int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if(user is null)
            return Result<List<JobApplicationResponse>>.Failure("User not found.", ErrorType.NotFound);

        var application = await unitOfWork.JobApplications.GetByApplicantIdAsync(currentUserId);

        var response = mapper.Map<List<JobApplicationResponse>>(application);

        return Result<List<JobApplicationResponse>>.Success(response);

    }

    public Task<Result> RejectAsync(int id, int currentUserId)
        => ChangeStatusAsync(id, currentUserId, "reject");

    public Task<Result> ScheduleInterviewAsync(int id, int currentUserId)
        => ChangeStatusAsync(id, currentUserId, "interview");

    public Task<Result> StartReviewAsync(int id, int currentUserId)
        => ChangeStatusAsync(id, currentUserId, "review");

    public Task<Result> AcceptAsync(int id, int currentUserId)
        => ChangeStatusAsync(id, currentUserId, "accept");

    public async Task<Result<JobApplicationResponse>> UpdateAsync(int id, UpdateJobApplicationRequest request, int currentUserId)
    {
        var application = await unitOfWork.JobApplications.GetByIdAsync(id);

        if (application is null)
            return Result<JobApplicationResponse>.Failure("Job application not found.", ErrorType.NotFound);

        if (application.ApplicantId != currentUserId)
            return Result<JobApplicationResponse>.Failure("You are not allowwed to update this job application.", ErrorType.Forbidden);

        var resumeUrl = request.ResumeUrl?.Trim() ?? string.Empty;
        var coverLetter = request.CoverLetter?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(resumeUrl)
            || string.IsNullOrWhiteSpace(coverLetter))
            return Result<JobApplicationResponse>.Failure("Resume URL and cover letter are required.", ErrorType.Validation);

        try
        {
            application.Update(resumeUrl, coverLetter, currentUserId);
        }
        catch (DomainException ex)
        {
            return Result<JobApplicationResponse>.Failure(ex.Message, ErrorType.Validation);
        }
        unitOfWork.JobApplications.Update(application);
        await unitOfWork.SaveChangesAsync();

        var response = mapper.Map<JobApplicationResponse>(application);

        return Result<JobApplicationResponse>.Success(response);
    }

    public async Task<Result> WithdrawAsync(int id, int currentUserId)
    {
        var application = await unitOfWork.JobApplications.GetByIdAsync(id);

        if(application is null)
            return Result.Failure("Job application not found.", ErrorType.NotFound);

        if(application.ApplicantId != currentUserId)
            return Result.Failure("You are not allowwed to withdraw this job application.", ErrorType.Forbidden);

        try
        {
            application.Withdraw(currentUserId);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ErrorType.Validation);
        }

        unitOfWork.JobApplications.Update(application);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Application withdrawn successfully.");
    }


    private async Task<Result> ChangeStatusAsync(int id, int currentUserId, string action)
    {
        var application = await unitOfWork.JobApplications.GetByIdAsync(id);

        if (application is null)
            return Result.Failure("Application not found.", ErrorType.NotFound);

        var job = await unitOfWork.Jobs.GetByIdAsync(application.JobId);

        if (job is null)
            return Result.Failure("Job not found.", ErrorType.NotFound);

        if (!await CanManageApplicationsAsync(job.CompanyId, currentUserId))
            return Result.Failure("You are not allowed to manage this application.",ErrorType.Forbidden);

        try
        {
            switch (action)
            {
                case "review":
                    application.StartReview(currentUserId);
                    break;

                case "interview":
                    application.ScheduleInterview(currentUserId);
                    break;

                case "accept":
                    application.Accept(currentUserId);
                    break;

                case "reject":
                    application.Reject(currentUserId);
                    break;

                default:
                    return Result.Failure("Invalid application action.", ErrorType.Validation);
            }
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ErrorType.Validation);
        }

        await CreateNotificationAsync(
            application.ApplicantId,
            "Application status updated",
            $"Your application for '{job.Title}' is now {application.Status}.",
            action switch
            {
                "review" => NotificationType.ApplicationStatusChanged,
                "interview" => NotificationType.InterviewInvitation,
                "accept" => NotificationType.ApplicationAccepted,
                "reject" => NotificationType.ApplicationRejected,
                _ => NotificationType.ApplicationStatusChanged
            });

        unitOfWork.JobApplications.Update(application);
        await unitOfWork.SaveChangesAsync();

        return Result.Success("Application status updated successfully.");
    }
    private async Task<bool> CanManageApplicationsAsync(int companyId, int currentUserId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(currentUserId);

        if (user is null)
            return false;

        if (user.Role == UserRole.Admin)
            return true;

        if (user.Role != UserRole.Recruiter)
            return false;

        var membership =
            await unitOfWork.CompanyMembers.GetByCompanyAndUserAsync(
                companyId, currentUserId);

        return membership is not null &&
            membership.Role is CompanyMemberRole.Owner
                or CompanyMemberRole.Recruiter;
    }
    private async Task CreateNotificationAsync(int userId,string title,string message,NotificationType type)
    {
        var notification = new Notification(userId,title,message,type);

        await unitOfWork.Notifications.AddAsync(notification);
    }
}
