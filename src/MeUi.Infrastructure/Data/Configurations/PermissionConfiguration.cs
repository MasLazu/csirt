using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ActionCode)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.DeletedAt);

        // Indexes
        builder.HasIndex(x => new { x.ResourceCode, x.ActionCode })
            .IsUnique();

        builder.HasIndex(x => x.DeletedAt);

        // Relationships are already configured in Resource and Action configurations
    }
}