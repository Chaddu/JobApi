using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CompanyMemberConfiguration : BaseEntityConfiguration<CompanyMember>
{
    public override void Configure(EntityTypeBuilder<CompanyMember> builder)
    {
        base.Configure(builder);

        builder.ToTable("CompanyMembers");

        builder.Property(member => member.Role)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(member => member.JoinedAt)
            .IsRequired();

        builder.HasOne(member => member.Company)
            .WithMany()
            .HasForeignKey(member => member.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(member => member.User)
            .WithMany()
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(member => new { member.CompanyId, member.UserId })
            .IsUnique();
    }
}