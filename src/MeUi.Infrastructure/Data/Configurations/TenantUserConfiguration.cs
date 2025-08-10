using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantUserConfiguration : IEntityTypeConfiguration<TenantUser>
{
    public void Configure(EntityTypeBuilder<TenantUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Username).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(255);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);

        builder.HasIndex(x => new { x.Username, x.TenantId }).IsUnique();
        builder.HasIndex(x => new { x.Email, x.TenantId }).IsUnique();
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.TenantUsers)
            .HasForeignKey(x => x.TenantId);

        builder.HasMany(x => x.TenantUserLoginMethods)
            .WithOne(x => x.TenantUser)
            .HasForeignKey(x => x.TenantUserId);

        builder.HasMany(x => x.TenantUserRefreshTokens)
            .WithOne(x => x.TenantUser)
            .HasForeignKey(x => x.TenantUserId);

        builder.HasMany(x => x.TenantUserRoles)
            .WithOne(x => x.TenantUser)
            .HasForeignKey(x => x.TenantUserId);
    }
}