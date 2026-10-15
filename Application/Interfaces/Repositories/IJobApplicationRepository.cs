using Domain.Entities;

namespace Application.Interfaces.Repositories;
public interface IJobApplicationRepository : IGenericRepository<JobApplication>
{ 
    Task<bool> HasAppliedAsync(int jobId, int applicantId);
    Task<List<JobApplication>> GetByJobIdAsync(int jobId);
    Task<List<JobApplication>> GetByApplicantIdAsync(int applicantId);
    Task<bool> HasAcceptedApplicationAsync(int companyId, int userId);
}
