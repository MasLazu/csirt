using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class TenantAsnConfiguration : IEntityTypeConfiguration<TenantAsnRegistry>
{
    public void Configure(EntityTypeBuilder<TenantAsnRegistry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.TenantId, x.AsnRegistryId }).IsUnique();
        builder.HasIndex(x => x.AsnRegistryId);
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.DeletedAt);

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.TenantAsnRegistries)
            .HasForeignKey(x => x.TenantId);

        builder.HasOne(x => x.AsnRegistry)
            .WithMany()
            .HasForeignKey(x => x.AsnRegistryId);
    }
}