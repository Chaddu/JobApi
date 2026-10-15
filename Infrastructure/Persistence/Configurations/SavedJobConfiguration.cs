using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SavedJobConfiguration : BaseEntityConfiguration<SavedJob>
{
    public override void Configure(EntityTypeBuilder<SavedJob> builder)
    {
        base.Configure(builder);

        builder.ToTable("SavedJobs");

        builder.HasOne(savedJob => savedJob.User)
            .WithMany()
            .HasForeignKey(savedJob => savedJob.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(savedJob => savedJob.Job)
            .WithMany()
            .HasForeignKey(savedJob => savedJob.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(savedJob => new
        {
            savedJob.UserId,
            savedJob.JobId
        }).IsUnique();
    }
}
