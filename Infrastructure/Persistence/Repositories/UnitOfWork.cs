using Application.Interfaces;
using Application.Interfaces.Repositories;

namespace Infrastructure.Persistence.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public IUserRepository Users { get; } = new UserRepository(context);
    public ICompanyRepository Companies { get; } = new CompanyRepository(context);
    public ICompanyMemberRepository CompanyMembers { get; } = new CompanyMemberRepository(context);
    public IJobRepository Jobs { get; } = new JobRepository(context);
    public IJobApplicationRepository JobApplications { get; } = new JobApplicationRepository(context);
    public ISavedJobRepository SavedJobs { get; } = new SavedJobRepository(context);
    public ISkillRepository Skills { get; } = new SkillRepository(context);
    public IUserSkillRepository UserSkills { get; } = new UserSkillRepository(context);
    public IJobSkillRepository JobSkills { get; } = new JobSkillRepository(context);
    public INotificationRepository Notifications { get; } = new NotificationRepository(context);
    public ICompanyReviewRepository CompanyReviews { get; } = new CompanyReviewRepository(context);

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}
