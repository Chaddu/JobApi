using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class JobApplicationConfiguration : BaseEntityConfiguration<JobApplication>
{
    public override void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        base.Configure(builder);

        builder.ToTable("JobApplications");

        builder.Property(application => application.ResumeUrl)
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(application => application.CoverLetter)
            .HasMaxLength(5000)
            .IsRequired();

        builder.Property(application => application.Status)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(application => application.AppliedAt)
            .IsRequired();

        builder.HasOne<Job>()
            .WithMany()
            .HasForeignKey(application => application.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(application => application.ApplicantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(application => new
        {
            application.JobId,
            application.ApplicantId
        }).IsUnique();

        builder.HasIndex(application => new
        {
            application.ApplicantId,
            application.Status
        });
    }
}
