using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class JobConfiguration : BaseEntityConfiguration<Job>
{
    public override void Configure(EntityTypeBuilder<Job> builder)
    {
        base.Configure(builder);

        builder.ToTable("Jobs");

        builder.Property(job => job.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(job => job.Description)
            .HasMaxLength(10000)
            .IsRequired();

        builder.Property(job => job.Location)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(job => job.SalaryMin)
            .HasPrecision(18, 2);

        builder.Property(job => job.SalaryMax)
            .HasPrecision(18, 2);

        builder.Property(job => job.EmploymentType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(job => job.ExperienceLevel)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(job => job.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(job => job.ApplicationDeadline);

        builder.Property(job => job.RowVersion)
    .IsRowVersion();

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(job => job.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(job => job.CompanyId);

        builder.HasIndex(job => new { job.Status, job.ApplicationDeadline });

        builder.HasIndex(job => job.CreatedAt);
    }
}
