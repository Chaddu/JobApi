using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class UserSkillConfiguration : BaseEntityConfiguration<UserSkill>
{
    public override void Configure(EntityTypeBuilder<UserSkill> builder)
    {
        base.Configure(builder);

        builder.ToTable("UserSkills");

        builder.Property(userSkill => userSkill.YearsOfExperience)
            .IsRequired();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(userSkill => userSkill.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(userSkill => userSkill.Skill)
            .WithMany(skill => skill.UserSkills)
            .HasForeignKey(userSkill => userSkill.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(userSkill => new
        {
            userSkill.UserId,
            userSkill.SkillId
        }).IsUnique();
    }
}
