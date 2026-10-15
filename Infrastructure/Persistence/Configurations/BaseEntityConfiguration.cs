using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity<int>
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.CreatedAt)
            .IsRequired();

        builder.Property(entity => entity.UpdatedAt);

        builder.Property(entity => entity.DeletedAt);

        builder.Property(entity => entity.CreatedById);

        builder.Property(entity => entity.UpdatedById);

        builder.Property(entity => entity.DeletedById);

        builder.HasQueryFilter(entity => !entity.DeletedAt.HasValue);
    }
}
