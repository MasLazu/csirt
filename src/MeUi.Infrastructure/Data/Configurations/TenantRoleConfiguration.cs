using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantRoleConfiguration : IEntityTypeConfiguration<TenantRole>
{
    public void Configure(EntityTypeBuilder<TenantRole> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.DeletedAt);
        builder.HasIndex(x => x.Name);

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.TenantRoles)
            .HasForeignKey(x => x.TenantId);

        builder.HasMany(x => x.TenantUserRoles)
            .WithOne(x => x.TenantRole)
            .HasForeignKey(x => x.TenantRoleId);

        builder.HasMany(x => x.TenantRolePermissions)
            .WithOne(x => x.TenantRole)
            .HasForeignKey(x => x.TenantRoleId);
    }
}