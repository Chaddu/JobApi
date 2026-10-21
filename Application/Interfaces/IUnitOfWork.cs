using Application.Interfaces.Repositories;


namespace Application.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ICompanyRepository Companies { get; }
    ICompanyMemberRepository CompanyMembers { get; }
    IJobRepository Jobs { get; }
    IJobApplicationRepository JobApplications { get; }
    ISavedJobRepository SavedJobs { get; }
    ISkillRepository Skills { get; }
    IUserSkillRepository UserSkills { get; }
    IJobSkillRepository JobSkills { get; }
    INotificationRepository Notifications { get; }
    ICompanyReviewRepository CompanyReviews { get; }

    Task<int> SaveChangesAsync();
    Task ExecuteInTransactionAsync(Func<Task> operation);
}