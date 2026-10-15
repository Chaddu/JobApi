using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;
public class Job : BaseEntity<int>
{
    public int CompanyId { get; private set; }
    public int CreatedById { get; private set; }

    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;

    public decimal? SalaryMin { get; private set; }
    public decimal? SalaryMax { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    public EmploymentType EmploymentType { get; private set; }
    public ExperienceLevel ExperienceLevel { get; private set; }
    public JobStatus Status { get; private set; }

    public DateTime? ApplicationDeadline { get; private set; }

    public Company Company { get; private set; } = null!;
    public User CreatedBy { get; private set; } = null!;

    private Job()
    {
    }

    public Job(
        int companyId,
        int createdById,
        string title,
        string description,
        string location,
        decimal? salaryMin,
        decimal? salaryMax,
        EmploymentType employmentType,
        ExperienceLevel experienceLevel,
        DateTime? applicationDeadline)
    {
        ValidateSalary(salaryMin, salaryMax);
        ValidateDeadline(applicationDeadline);

        CompanyId = companyId;
        CreatedById = createdById;
        Title = title;
        Description = description;
        Location = location;
        SalaryMin = salaryMin;
        SalaryMax = salaryMax;
        EmploymentType = employmentType;
        ExperienceLevel = experienceLevel;
        ApplicationDeadline = applicationDeadline;
        Status = JobStatus.Draft;
    }

    public void Update(
        string title,
        string description,
        string location,
        decimal? salaryMin,
        decimal? salaryMax,
        EmploymentType employmentType,
        ExperienceLevel experienceLevel,
        DateTime? applicationDeadline,
        int? updatedById = null)
    {
        ValidateSalary(salaryMin, salaryMax);
        ValidateDeadline(applicationDeadline);

        if (Status == JobStatus.Closed)
            throw new DomainException("A closed job cannot be updated.");

        Title = title;
        Description = description;
        Location = location;
        SalaryMin = salaryMin;
        SalaryMax = salaryMax;
        EmploymentType = employmentType;
        ExperienceLevel = experienceLevel;
        ApplicationDeadline = applicationDeadline;

        MarkAsUpdated(updatedById);
    }

    public void Publish(int? updatedById = null)
    {
        if (Status != JobStatus.Draft)
            throw new DomainException("Only draft jobs can be published.");

        ValidateDeadline(ApplicationDeadline);

        Status = JobStatus.Open;

        MarkAsUpdated(updatedById);
    }

    public void Close(int? updatedById = null)
    {
        if (Status != JobStatus.Open)
            throw new DomainException("Only open jobs can be closed.");

        Status = JobStatus.Closed;

        MarkAsUpdated(updatedById);
    }

    private static void ValidateSalary(decimal? salaryMin, decimal? salaryMax)
    {
        if (salaryMin.HasValue && salaryMin < 0)
            throw new DomainException("Minimum salary cannot be negative.");

        if (salaryMax.HasValue && salaryMax < 0)
            throw new DomainException("Maximum salary cannot be negative.");

        if (salaryMin.HasValue &&
            salaryMax.HasValue &&
            salaryMin > salaryMax)
        {
            throw new DomainException("Minimum salary cannot be greater than maximum salary.");
        }
    }

    private static void ValidateDeadline(DateTime? applicationDeadline)
    {
        if (applicationDeadline.HasValue &&
            applicationDeadline <= DateTime.UtcNow)
        {
            throw new DomainException("Application deadline must be in the future.");
        }
    }
}
