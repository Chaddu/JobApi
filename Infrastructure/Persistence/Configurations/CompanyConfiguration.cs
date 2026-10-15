using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CompanyConfiguration : BaseEntityConfiguration<Company>
{
    public override void Configure(EntityTypeBuilder<Company> builder)
    {
        base.Configure(builder);

        builder.ToTable("Companies");

        builder.Property(company => company.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(company => company.Description)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(company => company.Website)
            .HasMaxLength(500);

        builder.Property(company => company.Location)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(company => company.Name)
            .IsUnique();
    }
}