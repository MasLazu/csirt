using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ActionCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.DeletedAt);
        builder.HasIndex(x => new { x.ResourceCode, x.ActionCode }).IsUnique();

        builder.HasOne(x => x.Resource)
            .WithMany()
            .HasForeignKey(x => x.ResourceCode);

        builder.HasOne(x => x.Action)
            .WithMany()
            .HasForeignKey(x => x.ActionCode);
    }
}