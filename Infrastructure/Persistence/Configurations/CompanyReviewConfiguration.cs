using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class CompanyReviewConfiguration : BaseEntityConfiguration<CompanyReview>
{
    public override void Configure(EntityTypeBuilder<CompanyReview> builder)
    {
        base.Configure(builder);

        builder.ToTable("CompanyReviews");

        builder.Property(review => review.Rating)
            .IsRequired();

        builder.Property(review => review.Comment)
            .HasMaxLength(3000)
            .IsRequired();

        builder.HasOne(review => review.Company)
            .WithMany()
            .HasForeignKey(review => review.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(review => review.User)
            .WithMany()
            .HasForeignKey(review => review.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(review => new
        {
            review.CompanyId,
            review.UserId
        }).IsUnique();
    }
}