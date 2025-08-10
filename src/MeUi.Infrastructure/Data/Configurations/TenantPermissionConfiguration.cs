using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantPermissionConfiguration : IEntityTypeConfiguration<TenantPermission>
{
    public void Configure(EntityTypeBuilder<TenantPermission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ResourceCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.ActionCode).IsRequired().HasMaxLength(50);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => new { x.ResourceCode, x.ActionCode }).IsUnique();
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.Resource)
            .WithMany(x => x.TenantPermissions)
            .HasForeignKey(x => x.ResourceCode)
            .HasPrincipalKey(x => x.Code);

        builder.HasOne(x => x.Action)
            .WithMany(x => x.TenantPermissions)
            .HasForeignKey(x => x.ActionCode)
            .HasPrincipalKey(x => x.Code);

        builder.HasMany(x => x.PageTenantPermissions)
            .WithOne(x => x.TenantPermission)
            .HasForeignKey(x => x.TenantPermissionId);

        builder.HasMany(x => x.TenantRolePermissions)
            .WithOne(x => x.TenantPermission)
            .HasForeignKey(x => x.TenantPermissionId);
    }
}