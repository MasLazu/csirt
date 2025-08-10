using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantUserRoleConfiguration : IEntityTypeConfiguration<TenantUserRole>
{
    public void Configure(EntityTypeBuilder<TenantUserRole> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.TenantUserId, x.TenantRoleId }).IsUnique();
        builder.HasIndex(x => x.TenantUserId);
        builder.HasIndex(x => x.TenantRoleId);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.TenantUser)
            .WithMany(x => x.TenantUserRoles)
            .HasForeignKey(x => x.TenantUserId);

        builder.HasOne(x => x.TenantRole)
            .WithMany()
            .HasForeignKey(x => x.TenantRoleId);
    }
}