using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MeUi.Domain.Entities;

namespace MeUi.Infrastructure.Data.Configurations;

public class AsnRegistryConfiguration : IEntityTypeConfiguration<AsnRegistry>
{
    public void Configure(EntityTypeBuilder<AsnRegistry> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Number)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.HasIndex(e => e.Number);

        builder.Property(x => x.DeletedAt);

        builder.HasMany(e => e.ThreatEvents)
            .WithOne(t => t.AsnRegistry)
            .HasForeignKey(t => t.AsnRegistryId);
    }
}