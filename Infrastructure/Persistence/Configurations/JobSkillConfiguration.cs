using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class JobSkillConfiguration : BaseEntityConfiguration<JobSkill>
{
    public override void Configure(EntityTypeBuilder<JobSkill> builder)
    {
        base.Configure(builder);

        builder.ToTable("JobSkills");

        builder.Property(jobSkill => jobSkill.IsRequired)
            .IsRequired();

        builder.HasOne<Job>()
            .WithMany()
            .HasForeignKey(jobSkill => jobSkill.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(jobSkill => jobSkill.Skill)
            .WithMany(skill => skill.JobSkills)
            .HasForeignKey(jobSkill => jobSkill.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(jobSkill => new
        {
            jobSkill.JobId,
            jobSkill.SkillId
        }).IsUnique();
    }
}