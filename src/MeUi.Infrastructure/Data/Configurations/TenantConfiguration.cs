using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(255);
        builder.Property(x => x.ContactEmail).IsRequired().HasMaxLength(255);
        builder.Property(x => x.ContactPhone).HasMaxLength(50);

        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.ContactEmail);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasMany(x => x.TenantAsnRegistries)
            .WithOne(x => x.Tenant)
            .HasForeignKey(x => x.TenantId);

        builder.HasMany(x => x.TenantUsers)
            .WithOne(x => x.Tenant)
            .HasForeignKey(x => x.TenantId);

        builder.HasMany(x => x.TenantRoles)
            .WithOne(x => x.Tenant)
            .HasForeignKey(x => x.TenantId);
    }
}