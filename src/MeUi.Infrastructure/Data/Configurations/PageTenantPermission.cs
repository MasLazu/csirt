using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class PageTenantPermissionConfiguration : IEntityTypeConfiguration<PageTenantPermission>
{
    public void Configure(EntityTypeBuilder<PageTenantPermission> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.UpdatedAt);
        builder.Property(x => x.DeletedAt);

        builder.HasIndex(x => x.PageId);
        builder.HasIndex(x => x.TenantPermissionId);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.Page)
            .WithMany(x => x.PageTenantPermissions)
            .HasForeignKey(x => x.PageId);

        builder.HasOne(x => x.TenantPermission)
            .WithMany(x => x.PageTenantPermissions)
            .HasForeignKey(x => x.TenantPermissionId);
    }
}