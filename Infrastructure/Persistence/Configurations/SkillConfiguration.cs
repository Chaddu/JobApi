using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class SkillConfiguration : BaseEntityConfiguration<Skill>
{
    public override void Configure(EntityTypeBuilder<Skill> builder)
    {
        base.Configure(builder);

        builder.ToTable("Skills");

        builder.Property(skill => skill.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(skill => skill.Description)
            .HasMaxLength(1000)
            .IsRequired();

        builder.HasIndex(skill => skill.Name)
            .IsUnique();
    }
}
